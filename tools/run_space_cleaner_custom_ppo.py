from pathlib import Path
import sys

import mlagents.plugins as mla_plugins


PROJECT_ROOT = Path(__file__).resolve().parents[1]
PLUGIN_SRC = PROJECT_ROOT / "ml" / "space_cleaner_custom_ppo" / "src"

if str(PLUGIN_SRC) not in sys.path:
    sys.path.insert(0, str(PLUGIN_SRC))

from space_cleaner_custom_ppo.optimizer import SpaceCleanerPPOSettings
from space_cleaner_custom_ppo.trainer import SpaceCleanerPPOTrainer


mla_plugins.all_trainer_types[SpaceCleanerPPOTrainer.get_trainer_name()] = SpaceCleanerPPOTrainer
mla_plugins.all_trainer_settings[
    SpaceCleanerPPOTrainer.get_trainer_name()
] = SpaceCleanerPPOSettings


def main():
    from mlagents.trainers.learn import main as learn_main

    learn_main()


if __name__ == "__main__":
    main()
