# Space Cleaner Custom PPO

This package adds a repo-local ML-Agents trainer plugin for the space cleaning task.

What it changes:

- Keeps the standard PPO trainer flow from ML-Agents Release 23 / `mlagents==1.1.0`.
- Adds an optional continuous-action smoothness loss so the rocket is discouraged from jittering between very different thrust and turn commands on adjacent steps.
- Pairs with `config/space_cleaner_custom_ppo.yaml`, which also enables recurrent memory and a curriculum-driven `difficulty_level`.

Why this is project-specific:

- The current task is a long-horizon continuous-control problem: one rocket, `35` debris, a large arena, drifting targets, and only nearest-target observations.
- Prior training in `results/training_v1` reached a final mean cumulative reward of about `3.26` at `3,000,101` steps, which suggests the policy is learning something but is still far from consistently clearing the arena.
- This setup is a better fit for curriculum + memory than for raw PPO on the hardest scene from step 0.

Preferred launch path from the project root:

```powershell
& .\.venv\Scripts\python.exe .\tools\run_space_cleaner_custom_ppo.py .\config\space_cleaner_custom_ppo.yaml --run-id space_cleaner_custom_ppo_v1
```

Optional editable install if you want `mlagents-learn` to discover the trainer through entry points:

```powershell
& .\.venv\Scripts\python.exe -m pip install --no-build-isolation -e .\ml\space_cleaner_custom_ppo
& .\.venv\Scripts\mlagents-learn.exe .\config\space_cleaner_custom_ppo.yaml --run-id space_cleaner_custom_ppo_v1
```

Then press Play in the Unity Editor while `Assets/Solar More debris.unity` is open.
