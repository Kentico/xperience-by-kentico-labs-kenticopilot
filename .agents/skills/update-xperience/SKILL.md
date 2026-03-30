---
name: update-xperience
description: Automates NuGet and npm package updates for Xperience by Kentico projects, detects breaking changes, and updates version references in config. Use when updating an Xperience project to the latest version, or when user mentions "update", "upgrade", "latest version", or "breaking changes".
---

# Update Xperience Project

Complete all steps below autonomously unless the user prompts otherwise.

## Quick start

```
1. Detect current versions (Xperience NuGet + npm packages)
2. Find latest versions available via kentico-docs MCP - Changelog
3. Check for breaking changes or deprecated APIs between current and new versions
4. Update .csproj/Directory.Packages.props and package.json
5. Run dotnet/npm install
6. Validate compilation
7. Run `dotnet run -- --kxp-update` to update Xperience database
8. Update version refs in docs/config
```

## Full Workflow

### Phase 1: Discovery

**Step 1: Detect current versions**

- Read `.csproj` for Xperience NuGet version (e.g., `Kentico.Xperience.Core`)
- Read `package.json` for npm package version (e.g., `@kentico/xperience-*`)
- Run `dotnet package search Kentico.Xperience.Core --exact-match --format json` to find latest product version

**Step 2: Check breaking changes**

- Use `mcp_kentico-docs_kentico_docs_search` to query for Changelog details for the version updates
- Identify API removals, renamed methods, deprecated features
- Note migration steps required
- List any new required dependencies

### Phase 2: Update

- Ensure the application is not running and there are no uncommitted changes before proceeding with updates
- If these requirements are not met, stop and report to the user

**Step 3: Update NuGet packages**

- Update .NET files: `.csproj`/`Directory.Packages.props`
  - Replace current Xperience version with new version
- Run: `dotnet restore`

**Step 4: Update npm packages**

- Update `package.json`: Replace current @kentico/xperience-\* version with new version
- Run: `npm install`
- Check `package-lock.json` was updated
- If dependency resolution fails with non-trivial conflicts (peer deps, Node engine mismatch, transitive version deadlock), stop and report exact errors plus impacted packages

**Step 5: Update version references**

- Search for hardcoded version strings in:
  - README.md (feature/compatibility notes)
  - appsettings.json (if any version is documented)
  - CI/deployment configs (.github/workflows, etc.)
  - `mcp.json` for npm packaged mcp servers

### Phase 3: Validation

**Step 6: Compile & validate**

- Run: `dotnet build` (full rebuild, not just restore)
- Run: `npm run build` (if build script exists)
- Capture any compilation errors
- Check for deprecated API warnings

**Step 7: Verify breaking changes**

- Search codebase for removed/renamed APIs from breaking changes list
- Run code analysis or linter if available

**Step 8: Run automated tests**

- Run: `dotnet test` (unit/integration tests)
- Run: `npm test` (if test script exists)
- Follow documentation for custom test infrastructure (e.g. E2E tests)
- Verify all tests pass without errors

**Step 9: Document**

- Create summary of changes by category:
  - ✅ NuGet packages updated (list versions)
  - ✅ npm packages updated (list versions)
  - ⚠️ Breaking changes handled (list mitigations)
  - 📝 Files modified (list touched files)
- Suggest next steps (resolve API deprecations or removal, leverage new product features, implement recommended best practices)

## Advanced Features

### Handling Breaking Changes

When breaking changes detected:

1. **Auto-fixable** (renamed methods): Use search-replace or refactoring tools
2. **Manual review** (API removals): Flag for human decision, don't auto-remove
3. **Requires migration:** Link to kentico-docs for detailed migration guide

If breaking changes cannot be resolved:

1. Stop the update process
2. Report which APIs have changed
3. Link to the relevant Xperience by Kentico documentation, if available

### Dependency Conflict Policy

- Handle only straightforward version bumps that install cleanly
- If dependency issues are complex, do not attempt manual pinning chains or broad package churn
- Stop and report:
  - failed command output
  - conflicting packages and requested ranges
  - suggested next action for human review

## Use Cases

- **Routine update**: "Update Xperience to latest" → executes full workflow
- **Major version jump**: "Update from 30.12.0 to 31.3.0" → emphasizes breaking changes phase
- **Post-release review**: "Run update validation" → jumps to Phase 3
- **Version audit**: "What versions are we on?" → runs only Phase 1 discovery

## When to Use This Skill

✅ Updating Xperience packages for security patches, features, or bug fixes  
✅ Planning major version upgrades with breaking changes  
✅ Onboarding new team members (show current versions)

❌ Not for: Updating non-Xperience dependencies
❌ Not for: Downgrading versions
