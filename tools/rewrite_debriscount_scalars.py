"""Rewrite TensorBoard Environment/DebrisCount scalars from lesson numbers.

This fixes old result folders where the plotted debris count reflected the
scene's raw debris count instead of the curriculum lesson number. The rewritten
value is:

    Environment/Lesson Number/difficulty_level + 1

Original event files are copied to a backup directory before replacement.
"""

from __future__ import annotations

import argparse
import shutil
from datetime import datetime
from pathlib import Path

from tensorboard.backend.event_processing.event_file_loader import EventFileLoader
from tensorboard.backend.event_processing.event_accumulator import EventAccumulator
from tensorboard.summary.writer.record_writer import RecordWriter


DEBRIS_TAG = "Environment/DebrisCount"
LESSON_TAG = "Environment/Lesson Number/difficulty_level"


def scalar_values(event_path: Path, tag: str):
    accumulator = EventAccumulator(str(event_path), size_guidance={"scalars": 0})
    accumulator.Reload()
    if tag not in accumulator.Tags().get("scalars", []):
        return []
    return accumulator.Scalars(tag)


def lesson_value_for_step(lesson_by_step: dict[int, float], step: int) -> float | None:
    if step in lesson_by_step:
        return lesson_by_step[step]

    previous_steps = [lesson_step for lesson_step in lesson_by_step if lesson_step <= step]
    if not previous_steps:
        return None
    return lesson_by_step[max(previous_steps)]


def set_summary_scalar(summary_value, value: float) -> bool:
    value_kind = summary_value.WhichOneof("value")
    if value_kind == "simple_value":
        summary_value.simple_value = value
        return True

    if value_kind == "tensor":
        tensor = summary_value.tensor
        if tensor.float_val:
            tensor.float_val[:] = [value]
            return True
        if tensor.double_val:
            tensor.double_val[:] = [value]
            return True

    return False


def rewrite_event_file(event_path: Path, backup_root: Path, dry_run: bool) -> tuple[int, int]:
    debris_values = scalar_values(event_path, DEBRIS_TAG)
    lesson_values = scalar_values(event_path, LESSON_TAG)
    if not debris_values or not lesson_values:
        return 0, 0

    lesson_by_step = {event.step: event.value for event in lesson_values}
    changed_events = 0
    changed_values = 0
    replacement_path = event_path.with_name(event_path.name + ".rewrite_tmp")
    if replacement_path.exists():
        replacement_path.unlink()

    if dry_run:
        for event in EventFileLoader(str(event_path)).Load():
            if not event.HasField("summary"):
                continue
            replacement = lesson_value_for_step(lesson_by_step, event.step)
            if replacement is None:
                continue
            replacement += 1.0
            event_changed = False
            for summary_value in event.summary.value:
                if summary_value.tag == DEBRIS_TAG:
                    event_changed = True
                    changed_values += 1
            if event_changed:
                changed_events += 1
        return changed_events, changed_values

    try:
        with replacement_path.open("wb") as output:
            writer = RecordWriter(output)
            try:
                for event in EventFileLoader(str(event_path)).Load():
                    event_changed = False
                    if event.HasField("summary"):
                        replacement = lesson_value_for_step(lesson_by_step, event.step)
                        if replacement is not None:
                            replacement += 1.0
                            for summary_value in event.summary.value:
                                if summary_value.tag != DEBRIS_TAG:
                                    continue
                                if set_summary_scalar(summary_value, replacement):
                                    event_changed = True
                                    changed_values += 1
                    if event_changed:
                        changed_events += 1
                    writer.write(event.SerializeToString())
            finally:
                writer.close()

        relative_parent = event_path.resolve().parent.relative_to(Path.cwd().resolve())
        backup_dir = backup_root / relative_parent
        backup_dir.mkdir(parents=True, exist_ok=True)
        shutil.copy2(event_path, backup_dir / event_path.name)
        replacement_path.replace(event_path)
    finally:
        if replacement_path.exists():
            replacement_path.unlink()

    return changed_events, changed_values


def copy_or_rewrite_event_file(
    event_path: Path,
    output_path: Path,
    force_copy: bool,
    dry_run: bool,
) -> tuple[int, int, str]:
    debris_values = scalar_values(event_path, DEBRIS_TAG)
    lesson_values = scalar_values(event_path, LESSON_TAG)
    if force_copy or not debris_values or not lesson_values:
        if not dry_run:
            output_path.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(event_path, output_path)
        return 0, 0, "copied"

    lesson_by_step = {event.step: event.value for event in lesson_values}
    changed_events = 0
    changed_values = 0

    if dry_run:
        for event in EventFileLoader(str(event_path)).Load():
            if not event.HasField("summary"):
                continue
            replacement = lesson_value_for_step(lesson_by_step, event.step)
            if replacement is None:
                continue
            event_changed = False
            for summary_value in event.summary.value:
                if summary_value.tag == DEBRIS_TAG:
                    event_changed = True
                    changed_values += 1
            if event_changed:
                changed_events += 1
        return changed_events, changed_values, "rewritten"

    output_path.parent.mkdir(parents=True, exist_ok=True)
    with output_path.open("wb") as output:
        writer = RecordWriter(output)
        try:
            for event in EventFileLoader(str(event_path)).Load():
                event_changed = False
                if event.HasField("summary"):
                    replacement = lesson_value_for_step(lesson_by_step, event.step)
                    if replacement is not None:
                        replacement += 1.0
                        for summary_value in event.summary.value:
                            if summary_value.tag != DEBRIS_TAG:
                                continue
                            if set_summary_scalar(summary_value, replacement):
                                event_changed = True
                                changed_values += 1
                if event_changed:
                    changed_events += 1
                writer.write(event.SerializeToString())
        finally:
            writer.close()

    return changed_events, changed_values, "rewritten"


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--results-dir", type=Path, default=Path("results"))
    parser.add_argument("--skip-run", action="append", default=[])
    parser.add_argument("--dry-run", action="store_true")
    parser.add_argument("--backup-root", type=Path)
    parser.add_argument("--output-dir", type=Path)
    args = parser.parse_args()

    backup_root = args.backup_root
    if backup_root is None:
        stamp = datetime.now().strftime("%Y%m%d_%H%M%S")
        backup_root = Path("tensorboard_event_backups") / f"debriscount_{stamp}"

    total_events = 0
    total_values = 0
    skipped_runs = set(args.skip_run)
    for event_path in sorted(args.results_dir.rglob("events.out.tfevents*")):
        if event_path.name.endswith(".rewrite_tmp"):
            print(f"skip temp file: {event_path}")
            continue

        run_name = event_path.parents[1].name
        if args.output_dir is not None:
            output_path = args.output_dir / event_path.relative_to(args.results_dir)
            changed_events, changed_values, action = copy_or_rewrite_event_file(
                event_path,
                output_path,
                run_name in skipped_runs,
                args.dry_run,
            )
            if changed_values:
                print(f"{action} {run_name}: {changed_values} values in {changed_events} events")
            else:
                print(f"{action} {run_name}")
        else:
            if run_name in skipped_runs:
                print(f"skip {run_name}: {event_path}")
                continue

            changed_events, changed_values = rewrite_event_file(event_path, backup_root, args.dry_run)
            if changed_values:
                print(f"rewrite {run_name}: {changed_values} values in {changed_events} events")
            else:
                print(f"unchanged {run_name}")
        total_events += changed_events
        total_values += changed_values

    if args.dry_run:
        print(f"dry-run total: {total_values} values in {total_events} events")
    elif args.output_dir is not None:
        print(f"total: {total_values} values in {total_events} events")
        print(f"output: {args.output_dir}")
    else:
        print(f"total: {total_values} values in {total_events} events")
        print(f"backups: {backup_root}")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
