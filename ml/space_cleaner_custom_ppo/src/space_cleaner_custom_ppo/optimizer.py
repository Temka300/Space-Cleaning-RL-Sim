from typing import Dict, cast

import attr

from mlagents.torch_utils import torch
from mlagents_envs.timers import timed
from mlagents.trainers.buffer import AgentBuffer, BufferKey, RewardSignalUtil
from mlagents.trainers.policy.torch_policy import TorchPolicy
from mlagents.trainers.ppo.optimizer_torch import PPOSettings, TorchPPOOptimizer
from mlagents.trainers.settings import TrainerSettings
from mlagents.trainers.torch_entities.action_log_probs import ActionLogProbs
from mlagents.trainers.torch_entities.agent_action import AgentAction
from mlagents.trainers.torch_entities.utils import ModelUtils
from mlagents.trainers.trajectory import ObsUtil


@attr.s(auto_attribs=True)
class SpaceCleanerPPOSettings(PPOSettings):
    action_smoothness_coef: float = 0.0


class SpaceCleanerPPOOptimizer(TorchPPOOptimizer):
    def __init__(self, policy: TorchPolicy, trainer_settings: TrainerSettings):
        super().__init__(policy, trainer_settings)
        self.hyperparameters: SpaceCleanerPPOSettings = cast(
            SpaceCleanerPPOSettings, trainer_settings.hyperparameters
        )

    @timed
    def update(self, batch: AgentBuffer, num_sequences: int) -> Dict[str, float]:
        decay_lr = self.decay_learning_rate.get_value(self.policy.get_current_step())
        decay_eps = self.decay_epsilon.get_value(self.policy.get_current_step())
        decay_bet = self.decay_beta.get_value(self.policy.get_current_step())
        returns = {}
        old_values = {}
        for name in self.reward_signals:
            old_values[name] = ModelUtils.list_to_tensor(
                batch[RewardSignalUtil.value_estimates_key(name)]
            )
            returns[name] = ModelUtils.list_to_tensor(
                batch[RewardSignalUtil.returns_key(name)]
            )

        n_obs = len(self.policy.behavior_spec.observation_specs)
        current_obs = ObsUtil.from_buffer(batch, n_obs)
        current_obs = [ModelUtils.list_to_tensor(obs) for obs in current_obs]

        act_masks = ModelUtils.list_to_tensor(batch[BufferKey.ACTION_MASK])
        actions = AgentAction.from_buffer(batch)

        memories = [
            ModelUtils.list_to_tensor(batch[BufferKey.MEMORY][i])
            for i in range(0, len(batch[BufferKey.MEMORY]), self.policy.sequence_length)
        ]
        if len(memories) > 0:
            memories = torch.stack(memories).unsqueeze(0)

        value_memories = [
            ModelUtils.list_to_tensor(batch[BufferKey.CRITIC_MEMORY][i])
            for i in range(
                0, len(batch[BufferKey.CRITIC_MEMORY]), self.policy.sequence_length
            )
        ]
        if len(value_memories) > 0:
            value_memories = torch.stack(value_memories).unsqueeze(0)

        run_out = self.policy.actor.get_stats(
            current_obs,
            actions,
            masks=act_masks,
            memories=memories,
            sequence_length=self.policy.sequence_length,
        )

        log_probs = run_out["log_probs"]
        entropy = run_out["entropy"]

        values, _ = self.critic.critic_pass(
            current_obs,
            memories=value_memories,
            sequence_length=self.policy.sequence_length,
        )
        old_log_probs = ActionLogProbs.from_buffer(batch).flatten()
        log_probs = log_probs.flatten()
        loss_masks = ModelUtils.list_to_tensor(batch[BufferKey.MASKS], dtype=torch.bool)
        value_loss = ModelUtils.trust_region_value_loss(
            values, old_values, returns, decay_eps, loss_masks
        )
        policy_loss = ModelUtils.trust_region_policy_loss(
            ModelUtils.list_to_tensor(batch[BufferKey.ADVANTAGES]),
            log_probs,
            old_log_probs,
            loss_masks,
            decay_eps,
        )

        smoothness_loss = torch.tensor(0.0, device=log_probs.device)
        if (
            self.hyperparameters.action_smoothness_coef > 0.0
            and actions.continuous_tensor is not None
            and BufferKey.NEXT_CONT_ACTION in batch
        ):
            next_actions = ModelUtils.list_to_tensor(batch[BufferKey.NEXT_CONT_ACTION])
            transition_mask = loss_masks.float().reshape(-1)
            if BufferKey.DONE in batch:
                done_tensor = ModelUtils.list_to_tensor(
                    batch[BufferKey.DONE], dtype=torch.float32
                ).reshape(-1)
                transition_mask = transition_mask * (1.0 - done_tensor)
            action_delta = torch.square(actions.continuous_tensor - next_actions).mean(
                dim=-1
            )
            smoothness_loss = ModelUtils.masked_mean(action_delta, transition_mask)

        loss = (
            policy_loss
            + 0.5 * value_loss
            - decay_bet * ModelUtils.masked_mean(entropy, loss_masks)
            + self.hyperparameters.action_smoothness_coef * smoothness_loss
        )

        ModelUtils.update_learning_rate(self.optimizer, decay_lr)
        self.optimizer.zero_grad()
        loss.backward()
        self.optimizer.step()

        return {
            "Losses/Policy Loss": torch.abs(policy_loss).item(),
            "Losses/Value Loss": value_loss.item(),
            "Losses/Action Smoothness": smoothness_loss.item(),
            "Policy/Learning Rate": decay_lr,
            "Policy/Epsilon": decay_eps,
            "Policy/Beta": decay_bet,
        }
