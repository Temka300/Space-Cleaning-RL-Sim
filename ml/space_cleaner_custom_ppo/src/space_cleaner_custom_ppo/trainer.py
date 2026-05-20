from typing import Any, Dict, Tuple, cast

from mlagents.trainers.optimizer.torch_optimizer import TorchOptimizer
from mlagents.trainers.policy.torch_policy import TorchPolicy
from mlagents.trainers.ppo.trainer import PPOTrainer

from .optimizer import SpaceCleanerPPOOptimizer, SpaceCleanerPPOSettings


TRAINER_NAME = "spacecleaner_ppo"


class SpaceCleanerPPOTrainer(PPOTrainer):
    def __init__(
        self,
        behavior_name: str,
        reward_buff_cap: int,
        trainer_settings,
        training: bool,
        load: bool,
        seed: int,
        artifact_path: str,
    ):
        super().__init__(
            behavior_name,
            reward_buff_cap,
            trainer_settings,
            training,
            load,
            seed,
            artifact_path,
        )
        self.hyperparameters: SpaceCleanerPPOSettings = cast(
            SpaceCleanerPPOSettings, self.trainer_settings.hyperparameters
        )

    def create_optimizer(self) -> TorchOptimizer:
        return SpaceCleanerPPOOptimizer(cast(TorchPolicy, self.policy), self.trainer_settings)

    @staticmethod
    def get_trainer_name() -> str:
        return TRAINER_NAME


def get_type_and_setting() -> Tuple[Dict[str, Any], Dict[str, Any]]:
    return (
        {SpaceCleanerPPOTrainer.get_trainer_name(): SpaceCleanerPPOTrainer},
        {SpaceCleanerPPOTrainer.get_trainer_name(): SpaceCleanerPPOSettings},
    )
