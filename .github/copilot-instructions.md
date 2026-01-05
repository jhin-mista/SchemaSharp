# Instructions for GitHub and VisualStudio Copilot

## General

* Make only high confidence suggestions when reviewing code changes.
* When proposing changes or making edits yourself, you keep the SOLID design principles in mind and try to apply them where possible.
* Changes and additions should be testable and include new or adjusted tests.
* Always use the latest version C#.
* Files must have CRLF line endings.
* If you are unsure on how to proceed, ask the user for input instead of making something up.
* The code style should match the rest of the code base.
* Changes should not generate new warnings or errors.
* Classes should be sealed by default.

## Formatting

* Apply code-formatting style defined in `.editorconfig`.
* Prefer file-scoped namespace declarations and single-line using directives.
* Insert a newline before the opening curly brace of any code block (e.g., after `if`, `for`, `while`, `foreach`, `using`, `try`, etc.).
* Ensure that the final return statement of a method is on its own line.
* Use pattern matching and switch expressions wherever possible.
* Use `nameof` instead of string literals when referring to member names.
* Use collection expressions wherever possible.

### Nullable Reference Types

* Declare variables non-nullable, and check for `null` at entry points.
* Always use `is null` or `is not null` instead of `== null` or `!= null`.
* Trust the C# null annotations and don't add null checks when the type system says a value cannot be null.

### Testing

* We use NUnit SDK
* Use the "arrange/act/assert" pattern for writing tests and emit corresponding lowercase comments to highlight the sections.
* We use Moq for mocking.
* Assertions and mock verifications should always come last. They should not be in the arrange or act section.
* Mock setups should always be done with corresponding `It.IsAny<>()` calls for method parameters. They will exactly be verified during assertion.
* Assertions should be done with the `Assert.That()` method from NUnit.
* Use the pattern "MethodUnderTest_WhenUseCase_ShouldExpectedResult" for naming tests.