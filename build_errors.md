# Build Errors

Build attempted: whole solution

Summary: Build failed. Workspace had total of 70 errors but only 50 are shown below. Fix these and run build again to reveal remaining errors.

---

1. File: QuizMakerEngine\QuizzesController.cs (line 2)
```csharp
using Microsoft.AspNetCore.Mvc;
```
Error: CS0234: The type or namespace name 'AspNetCore' does not exist in the namespace 'Microsoft' (are you missing an assembly reference?)

2. File: QuizMakerEngine\QuizzesController.cs (line 14)
```csharp
public IFormFile File { get; set; } = null!;
```
Error: CS0246: The type or namespace name 'IFormFile' could not be found (are you missing a using directive or an assembly reference?)

3. File: QuizMakerEngine\QuizzesController.cs (line 19)
```csharp
public class QuizzesController : ControllerBase
```
Error: CS0246: The type or namespace name 'ControllerBase' could not be found (are you missing a using directive or an assembly reference?)

4. File: QuizMakerEngine\QuizzesController.cs (line 17)
```csharp
[ApiController]
```
Error: CS0246: The type or namespace name 'ApiControllerAttribute' could not be found (are you missing a using directive or an assembly reference?)

5. File: QuizMakerEngine\QuizzesController.cs (line 17)
```csharp
[ApiController]
```
Error: CS0246: The type or namespace name 'ApiController' could not be found (are you missing a using directive or an assembly reference?)

6. File: QuizMakerEngine\QuizzesController.cs (line 18)
```csharp
[Route("api/[controller]")]
```
Error: CS0246: The type or namespace name 'RouteAttribute' could not be found (are you missing a using directive or an assembly reference?)

7. File: QuizMakerEngine\QuizzesController.cs (line 18)
```csharp
[Route("api/[controller]")]
```
Error: CS0246: The type or namespace name 'Route' could not be found (are you missing a using directive or an assembly reference?)

8. File: QuizMakerEngine\QuizzesController.cs (line 22)
```csharp
private readonly IConfiguration _config;
```
Error: CS0246: The type or namespace name 'IConfiguration' could not be found (are you missing a using directive or an assembly reference?)

9. File: QuizMakerEngine\QuizzesController.cs (line 24)
```csharp
public QuizzesController(IQuizService quizzes, IConfiguration config)
```
Error: CS0246: The type or namespace name 'IConfiguration' could not be found (are you missing a using directive or an assembly reference?)

10. File: QuizMakerEngine\QuizzesController.cs (line 33)
```csharp
public async Task<IActionResult> GetAll() => Ok(await _quizzes.GetAllAsync());
```
Error: CS0246: The type or namespace name 'IActionResult' could not be found (are you missing a using directive or an assembly reference?)

11. File: QuizMakerEngine\QuizzesController.cs (line 39)
```csharp
public async Task<IActionResult> GetById(int id)
```
Error: CS0246: The type or namespace name 'IActionResult' could not be found (are you missing a using directive or an assembly reference?)

12. File: QuizMakerEngine\QuizzesController.cs (line 49)
```csharp
public async Task<IActionResult> Create([FromBody] CreateQuizDto dto)
```
Error: CS0246: The type or namespace name 'IActionResult' could not be found (are you missing a using directive or an assembly reference?)

13. File: QuizMakerEngine\QuizzesController.cs (line 68)
```csharp
public async Task<IActionResult> Generate([FromForm] GenerateQuizRequest request)
```
Error: CS0246: The type or namespace name 'IActionResult' could not be found (are you missing a using directive or an assembly reference?)

14. File: QuizMakerEngine\QuizzesController.cs (line 105)
```csharp
public async Task<IActionResult> Update(int id, [FromBody] UpdateQuizDto dto)
```
Error: CS0246: The type or namespace name 'IActionResult' could not be found (are you missing a using directive or an assembly reference?)

15. File: QuizMakerEngine\QuizzesController.cs (line 122)
```csharp
public async Task<IActionResult> Delete(int id) =>
```
Error: CS0246: The type or namespace name 'IActionResult' could not be found (are you missing a using directive or an assembly reference?)

16. File: QuizMakerEngine\QuizzesController.cs (line 31)
```csharp
[HttpGet]
```
Error: CS0246: The type or namespace name 'HttpGetAttribute' could not be found (are you missing a using directive or an assembly reference?)

17. File: QuizMakerEngine\QuizzesController.cs (line 31)
```csharp
[HttpGet]
```
Error: CS0246: The type or namespace name 'HttpGet' could not be found (are you missing a using directive or an assembly reference?)

18. File: QuizMakerEngine\QuizzesController.cs (line 32)
```csharp
[ProducesResponseType(typeof(List<QuizSummaryDto>), StatusCodes.Status200OK)]
```
Error: CS0246: The type or namespace name 'ProducesResponseTypeAttribute' could not be found (are you missing a using directive or an assembly reference?)

19. File: QuizMakerEngine\QuizzesController.cs (line 32)
```csharp
[ProducesResponseType(typeof(List<QuizSummaryDto>), StatusCodes.Status200OK)]
```
Error: CS0246: The type or namespace name 'ProducesResponseType' could not be found (are you missing a using directive or an assembly reference?)

20. File: QuizMakerEngine\QuizzesController.cs (line 32)
```csharp
[ProducesResponseType(typeof(List<QuizSummaryDto>), StatusCodes.Status200OK)]
```
Error: CS0103: The name 'StatusCodes' does not exist in the current context

21. File: QuizMakerEngine\QuizzesController.cs (line 36)
```csharp
[HttpGet("{id:int}")]
```
Error: CS0246: The type or namespace name 'HttpGetAttribute' could not be found (are you missing a using directive or an assembly reference?)

22. File: QuizMakerEngine\QuizzesController.cs (line 36)
```csharp
[HttpGet("{id:int}")]
```
Error: CS0246: The type or namespace name 'HttpGet' could not be found (are you missing a using directive or an assembly reference?)

23. File: QuizMakerEngine\QuizzesController.cs (line 37)
```csharp
[ProducesResponseType(typeof(QuizDetailDto), StatusCodes.Status200OK)]
```
Error: CS0246: The type or namespace name 'ProducesResponseTypeAttribute' could not be found (are you missing a using directive or an assembly reference?)

24. File: QuizMakerEngine\QuizzesController.cs (line 37)
```csharp
[ProducesResponseType(typeof(QuizDetailDto), StatusCodes.Status200OK)]
```
Error: CS0246: The type or namespace name 'ProducesResponseType' could not be found (are you missing a using directive or an assembly reference?)

25. File: QuizMakerEngine\QuizzesController.cs (line 38)
```csharp
[ProducesResponseType(StatusCodes.Status404NotFound)]
```
Error: CS0246: The type or namespace name 'ProducesResponseTypeAttribute' could not be found (are you missing a using directive or an assembly reference?)

26. File: QuizMakerEngine\QuizzesController.cs (line 38)
```csharp
[ProducesResponseType(StatusCodes.Status404NotFound)]
```
Error: CS0246: The type or namespace name 'ProducesResponseType' could not be found (are you missing a using directive or an assembly reference?)

27. File: QuizMakerEngine\QuizzesController.cs (line 37)
```csharp
[ProducesResponseType(typeof(QuizDetailDto), StatusCodes.Status200OK)]
```
Error: CS0103: The name 'StatusCodes' does not exist in the current context

28. File: QuizMakerEngine\QuizzesController.cs (line 38)
```csharp
[ProducesResponseType(StatusCodes.Status404NotFound)]
```
Error: CS0103: The name 'StatusCodes' does not exist in the current context

29. File: QuizMakerEngine\QuizzesController.cs (line 46)
```csharp
[HttpPost]
```
Error: CS0246: The type or namespace name 'HttpPostAttribute' could not be found (are you missing a using directive or an assembly reference?)

30. File: QuizMakerEngine\QuizzesController.cs (line 46)
```csharp
[HttpPost]
```
Error: CS0246: The type or namespace name 'HttpPost' could not be found (are you missing a using directive or an assembly reference?)

31. File: QuizMakerEngine\QuizzesController.cs (line 47)
```csharp
[ProducesResponseType(typeof(QuizDetailDto), StatusCodes.Status201Created)]
```
Error: CS0246: The type or namespace name 'ProducesResponseTypeAttribute' could not be found (are you missing a using directive or an assembly reference?)

32. File: QuizMakerEngine\QuizzesController.cs (line 47)
```csharp
[ProducesResponseType(typeof(QuizDetailDto), StatusCodes.Status201Created)]
```
Error: CS0246: The type or namespace name 'ProducesResponseType' could not be found (are you missing a using directive or an assembly reference?)

33. File: QuizMakerEngine\QuizzesController.cs (line 48)
```csharp
[ProducesResponseType(StatusCodes.Status400BadRequest)]
```
Error: CS0246: The type or namespace name 'ProducesResponseTypeAttribute' could not be found (are you missing a using directive or an assembly reference?)

34. File: QuizMakerEngine\QuizzesController.cs (line 48)
```csharp
[ProducesResponseType(StatusCodes.Status400BadRequest)]
```
Error: CS0246: The type or namespace name 'ProducesResponseType' could not be found (are you missing a using directive or an assembly reference?)

35. File: QuizMakerEngine\QuizzesController.cs (line 47)
```csharp
[ProducesResponseType(typeof(QuizDetailDto), StatusCodes.Status201Created)]
```
Error: CS0103: The name 'StatusCodes' does not exist in the current context

36. File: QuizMakerEngine\QuizzesController.cs (line 48)
```csharp
[ProducesResponseType(StatusCodes.Status400BadRequest)]
```
Error: CS0103: The name 'StatusCodes' does not exist in the current context

37. File: QuizMakerEngine\QuizzesController.cs (line 49)
```csharp
public async Task<IActionResult> Create([FromBody] CreateQuizDto dto)
```
Error: CS0246: The type or namespace name 'FromBodyAttribute' could not be found (are you missing a using directive or an assembly reference?)

38. File: QuizMakerEngine\QuizzesController.cs (line 49)
```csharp
public async Task<IActionResult> Create([FromBody] CreateQuizDto dto)
```
Error: CS0246: The type or namespace name 'FromBody' could not be found (are you missing a using directive or an assembly reference?)

39. File: QuizMakerEngine\QuizzesController.cs (line 63)
```csharp
[HttpPost("generate")]
```
Error: CS0246: The type or namespace name 'HttpPostAttribute' could not be found (are you missing a using directive or an assembly reference?)

40. File: QuizMakerEngine\QuizzesController.cs (line 63)
```csharp
[HttpPost("generate")]
```
Error: CS0246: The type or namespace name 'HttpPost' could not be found (are you missing a using directive or an assembly reference?)

41. File: QuizMakerEngine\QuizzesController.cs (line 64)
```csharp
[Consumes("multipart/form-data")]
```
Error: CS0246: The type or namespace name 'ConsumesAttribute' could not be found (are you missing a using directive or an assembly reference?)

42. File: QuizMakerEngine\QuizzesController.cs (line 64)
```csharp
[Consumes("multipart/form-data")]
```
Error: CS0246: The type or namespace name 'Consumes' could not be found (are you missing a using directive or an assembly reference?)

43. File: QuizMakerEngine\QuizzesController.cs (line 65)
```csharp
[RequestSizeLimit(30_000_000)]
```
Error: CS0246: The type or namespace name 'RequestSizeLimitAttribute' could not be found (are you missing a using directive or an assembly reference?)

44. File: QuizMakerEngine\QuizzesController.cs (line 65)
```csharp
[RequestSizeLimit(30_000_000)]
```
Error: CS0246: The type or namespace name 'RequestSizeLimit' could not be found (are you missing a using directive or an assembly reference?)

45. File: QuizMakerEngine\QuizzesController.cs (line 66)
```csharp
[ProducesResponseType(typeof(QuizDetailDto), StatusCodes.Status201Created)]
```
Error: CS0246: The type or namespace name 'ProducesResponseTypeAttribute' could not be found (are you missing a using directive or an assembly reference?)

46. File: QuizMakerEngine\QuizzesController.cs (line 66)
```csharp
[ProducesResponseType(typeof(QuizDetailDto), StatusCodes.Status201Created)]
```
Error: CS0246: The type or namespace name 'ProducesResponseType' could not be found (are you missing a using directive or an assembly reference?)

47. File: QuizMakerEngine\QuizzesController.cs (line 67)
```csharp
[ProducesResponseType(StatusCodes.Status400BadRequest)]
```
Error: CS0246: The type or namespace name 'ProducesResponseTypeAttribute' could not be found (are you missing a using directive or an assembly reference?)

48. File: QuizMakerEngine\QuizzesController.cs (line 67)
```csharp
[ProducesResponseType(StatusCodes.Status400BadRequest)]
```
Error: CS0246: The type or namespace name 'ProducesResponseType' could not be found (are you missing a using directive or an assembly reference?)

49. File: QuizMakerEngine\QuizzesController.cs (line 66)
```csharp
[ProducesResponseType(typeof(QuizDetailDto), StatusCodes.Status201Created)]
```
Error: CS0103: The name 'StatusCodes' does not exist in the current context

50. File: QuizMakerEngine\QuizzesController.cs (line 67)
```csharp
[ProducesResponseType(StatusCodes.Status400BadRequest)]
```
Error: CS0103: The name 'StatusCodes' does not exist in the current context

---

Next steps: Fix missing ASP.NET Core references/usings or adjust project SDK/TargetFramework. After fixing, run the build again to get remaining errors.
