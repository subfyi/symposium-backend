#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/.."
mode=patch
version=
publish=false
while (($#)); do
  case "$1" in
    --major) mode=major ;;
    --minor) mode=minor ;;
    --patch) mode=patch ;;
    --version)
      shift
      version="${1:?Expected a version after --version}"
      ;;
    --push) publish=true ;;
    --dry-run) publish=false ;;
    --help)
      printf 'Usage: scripts/release.sh [--patch|--minor|--major|--version X.Y.Z] [--dry-run|--push]\n'
      exit 0
      ;;
    *) printf 'Unknown option: %s\n' "$1" >&2; exit 1 ;;
  esac
  shift
done

[[ -z "$(git status --porcelain)" ]] || { echo 'Commit or stash changes before releasing.' >&2; exit 1; }
branch="$(git branch --show-current)"
[[ -n "$branch" ]] || { echo 'Release from a branch, not detached HEAD.' >&2; exit 1; }

if "$publish"; then
  git fetch origin --tags
  upstream="$(git rev-parse --abbrev-ref --symbolic-full-name '@{upstream}')"
  [[ "$upstream" == "origin/$branch" ]] || { echo 'The branch must track origin before releasing.' >&2; exit 1; }
  [[ "$(git rev-parse HEAD)" == "$(git rev-parse "$upstream")" ]] || { echo 'Push/synchronize the branch before releasing.' >&2; exit 1; }
fi

latest=
while IFS= read -r candidate; do
  if [[ "$candidate" =~ ^v(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$ ]]; then
    latest="$candidate"
    break
  fi
done < <(git tag --sort=-version:refname)

if [[ -z "$version" ]]; then
  base="${latest#v}"
  IFS=. read -r major minor patch <<< "${base:-0.0.0}"
  case "$mode" in
    major) major=$((major + 1)); minor=0; patch=0 ;;
    minor) minor=$((minor + 1)); patch=0 ;;
    patch) patch=$((patch + 1)) ;;
  esac
  version="$major.$minor.$patch"
fi
[[ "$version" =~ ^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$ ]] || { echo 'Expected a stable semantic version X.Y.Z.' >&2; exit 1; }
tag="v$version"
if git show-ref --verify --quiet "refs/tags/$tag"; then
  printf 'Tag already exists: %s\n' "$tag" >&2
  exit 1
fi
printf 'Previous version: %s\nNext version: %s\nCommit: %s\n' "${latest:-none}" "$tag" "$(git rev-parse HEAD)"
if ! "$publish"; then
  echo 'Dry run only. No tag or remote changes. Pass --push to trigger deployment.'
  exit 0
fi
git tag -a "$tag" -m "Release $tag"
if ! git push origin "refs/tags/$tag"; then
  printf 'Push failed. Local tag retained; retry with: git push origin refs/tags/%s\n' "$tag" >&2
  exit 1
fi
printf 'Published %s. Follow the Deploy Backend workflow in Gitea Actions.\n' "$tag"
