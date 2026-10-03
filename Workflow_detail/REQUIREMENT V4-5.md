Đúng. Nếu **v3 đã chạy ổn**, thì mình sẽ đi **v4 → v5** thay vì tiếp tục nhồi thêm feature business. Đây là lúc biến School Simulator từ một bài OOP thành một project có nhiều điểm để nói trong phỏng vấn .NET.

Điểm quan trọng: **v4 và v5 nên được thiết kế như hai bước nâng cấp kiến trúc**, không phải chỉ "thêm JSON" và "thêm async".

---

# V4 — Persistence & Recovery

### Mục tiêu

Hiện tại:

```text
Application
    ↓
Memory
    ↓
Application exits
    ↓
💀 Everything disappears
```

V4 biến nó thành:

```text
Application
      ↓
  Repository
      ↓
   Memory
      ↓
Persistence Layer
      ↓
JSON / CSV
```

Khi restart:

```text
JSON
 ↓
Repository
 ↓
Memory
 ↓
Application continues
```

---

## 1. Persistence

Mình khuyên **JSON là format chính**.

Ví dụ:

```text
data/
├── students.json
├── teachers.json
├── subjects.json
└── exams.json
```

Ví dụ:

```json
{
    "id": 1,
    "name": "Alice",
    "age": 20,
    "gpa": 3.45
}
```

Bạn sẽ học:

* `System.Text.Json`
* Serialization
* Deserialization
* File I/O
* `Stream`
* `FileStream`
* async file I/O
* xử lý file không tồn tại
* corrupted JSON
* versioning cơ bản

---

# 2. Repository trở nên thực sự hữu ích

V3 có thể đang có kiểu:

```csharp
studentRepository.Add(student);
studentRepository.GetAll();
```

V4 nâng lên:

```csharp
public interface IRepository<T>
{
    Task AddAsync(T entity);
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

Sau đó:

```text
IRepository<T>
       │
       ├── InMemoryRepository<T>
       │
       └── JsonRepository<T>
```

Đây là một bước **rất đáng giá cho phỏng vấn**.

Bạn có thể giải thích:

> "Business logic không biết dữ liệu được lưu ở đâu. Nó chỉ phụ thuộc vào IRepository."

Đó chính là tư duy **Dependency Inversion**.

---

# 3. Application Recovery

Khi mở chương trình:

```text
Loading school data...

✓ Students loaded: 50
✓ Teachers loaded: 10
✓ Subjects loaded: 8

School loaded successfully.
```

Nếu file không tồn tại:

```text
students.json not found.

Creating new student repository...
```

Nếu JSON hỏng:

```text
Failed to load students.json.

Using empty repository.
```

Đây là nơi bạn bắt đầu học **Exception Handling** đúng cách.

---

# 4. Save Strategy

Một vấn đề thú vị:

Khi nào save?

### Option A

Sau mỗi mutation:

```text
Add Student
    ↓
Save JSON
```

### Option B

Save khi application shutdown:

```text
Application
    ↓
Memory
    ↓
Shutdown
    ↓
Save everything
```

### Option C — tốt hơn

Có một `PersistenceService`:

```text
Application
     │
     ↓
PersistenceService
     │
 ┌───┴────┐
 ↓        ↓
Student  Teacher
Repository
```

Bạn sẽ phải suy nghĩ về:

* consistency
* performance
* data loss
* transaction-like behavior

Đây chính là loại câu hỏi architectural thinking khá tốt khi phỏng vấn.

---

# 5. V4 nên thêm Backup

Một feature nhỏ nhưng rất đáng học:

```text
data/
├── students.json
├── teachers.json
└── backup/
    ├── students_20260916.json
    └── teachers_20260916.json
```

Trước khi overwrite:

```text
students.json
      ↓
backup
      ↓
write new students.json
```

Bạn sẽ bắt đầu gặp vấn đề:

> "Nếu application crash giữa lúc đang ghi file thì sao?"

Đây là một câu hỏi rất thực tế.

---

# V5 — Concurrency & Background Worker

Đây mới là phần mình nghĩ **đáng làm nhất cho phỏng vấn**.

Hiện tại:

```text
Student 1
   ↓
Take Exam
   ↓
Student 2
   ↓
Take Exam
   ↓
Student 3
```

Mọi thứ synchronous.

V5:

```text
             Exam
              │
      ┌───────┼────────┐
      ↓       ↓        ↓
 Student1 Student2 Student3
      │       │        │
      └───────┼────────┘
              ↓
          Results
```

Các student có thể làm bài "đồng thời".

---

# 1. Task / async / await

Bạn sẽ chuyển:

```csharp
student.TakeExam();
```

thành:

```csharp
await student.TakeExamAsync();
```

Và:

```csharp
var tasks = students.Select(
    student => student.TakeExamAsync(exam)
);

await Task.WhenAll(tasks);
```

Đây là một bài học cực kỳ quan trọng cho .NET backend.

Bạn sẽ hiểu:

* `Task`
* `async`
* `await`
* `Task.WhenAll`
* concurrency vs parallelism
* asynchronous I/O
* thread pool

---

# 2. Nhưng đừng chỉ random delay

Ví dụ:

```csharp
await Task.Delay(random.Next(1000, 5000));
```

để giả lập học sinh làm bài.

Sau đó:

```text
Student Alice started exam
Student Bob started exam
Student Charlie started exam

...

Student Bob finished
Student Alice finished
Student Charlie finished
```

Thứ tự finish khác thứ tự start.

Đây là lúc project bắt đầu **trông giống một hệ thống thật**.

---

# 3. Background Worker

Phần này cực kỳ đáng làm.

Bạn đã có ý tưởng:

> log/notification giống một worker bên ngoài.

V5 biến nó thành worker thật.

Ví dụ:

```text
Main Application
       │
       ├───────────────┐
       ↓               ↓
   School Engine    Event Queue
                       │
             ┌─────────┴─────────┐
             ↓                   ↓
        Logging Worker     Notification Worker
```

Trong .NET bạn có thể dùng:

```csharp
BackgroundService
```

hoặc đơn giản hơn trước:

```csharp
Task.Run(...)
```

Sau đó mới chuyển sang `BackgroundService`.

---

# 4. Queue

Đây là feature mình **rất khuyến khích**.

Khi student hoàn thành:

```text
Student completed exam
          ↓
      Event/Message
          ↓
      Queue
          ↓
   Worker processes
       ↙       ↘
    Logger   Notification
```

Bạn có thể dùng:

```csharp
Channel<T>
```

của .NET.

Ví dụ conceptually:

```csharp
Channel<SchoolEvent>
```

Producer:

```text
Exam completed
      ↓
Write to Channel
```

Consumer:

```text
Background Worker
      ↓
Read Channel
      ↓
Process event
```

Đây là kiến thức rất gần với backend production.

---

# 5. Race Condition

Đây là **boss fight của V5**.

Giả sử:

```text
Student A finishes
Student B finishes
Student C finishes
```

Cả ba cùng update:

```text
Teacher GPA
Teacher Salary
```

Nếu code không an toàn:

```text
Teacher Salary = 10000

A reads 10000
B reads 10000

A → 10500
B → 10500

Expected:
11000

Actual:
10500
```

💀

Bạn sẽ gặp:

* race condition
* shared state
* thread safety
* `lock`
* `Interlocked`
* concurrent collections

Ví dụ:

```csharp
lock (_salaryLock)
{
    _salary += bonus;
}
```

Đây là **một câu phỏng vấn rất hay**:

> "What happens if two requests modify the same resource at the same time?"

Sau V5 bạn có thể trả lời bằng kinh nghiệm từ chính project.

---

# 6. CancellationToken

Worker không nên chạy mãi.

Bạn sẽ học:

```csharp
CancellationToken
```

Ví dụ:

```text
Application started
      ↓
Worker running
      ↓
Processing events
      ↓
Ctrl + C
      ↓
Cancellation requested
      ↓
Worker finishes current task
      ↓
Graceful shutdown
```

Đây là một concept rất quan trọng khi làm server/background service.

---

# 7. Graceful Shutdown

Kết hợp:

```text
CancellationToken
+
BackgroundService
+
Persistence
```

Khi app shutdown:

```text
Stop accepting new work
        ↓
Finish existing work
        ↓
Flush queue
        ↓
Save data
        ↓
Stop workers
        ↓
Exit
```

Lúc này School Simulator đã bắt đầu có lifecycle giống một server thực sự.

---

# Và đây là thứ bạn sẽ học được cho phỏng vấn

Mình sẽ chia thành 4 tầng.

## 🟢 Junior C# questions

Sau V4/V5 bạn có thể nói về:

* Class / Object
* Encapsulation
* Inheritance
* Polymorphism
* Abstract class
* Interface
* Generics
* Collections
* Exception handling
* LINQ
* `async/await`
* `Task`
* `CancellationToken`

---

# 🟡 .NET questions

Bạn sẽ có kinh nghiệm thực tế với:

```text
System.Text.Json
System.IO
Stream
FileStream
Task
async/await
Channel<T>
BackgroundService
Dependency Injection
Configuration
Logging
```

Đây là những thứ rất đáng học trước ASP.NET Core.

---

# 🟠 Software Design

V4/V5 sẽ giúp bạn hiểu sâu hơn:

```text
SOLID
   ↓
Dependency Injection
   ↓
Repository
   ↓
Strategy
   ↓
Factory
   ↓
Observer/Event
   ↓
Producer/Consumer
```

Quan trọng nhất là bạn **không học pattern theo kiểu thuộc lòng**.

Bạn sẽ gặp vấn đề trước:

> "Tại sao nhiều student cùng update salary lại lỗi?"

rồi mới học:

> "À, đây là concurrency problem."

Đó là cách học tốt hơn rất nhiều.

---

# 🔴 System / Backend Interview

Đây là phần project bắt đầu giúp bạn trả lời các câu hỏi kiểu:

### "Synchronous và asynchronous khác nhau thế nào?"

Bạn có implementation thật.

### "Task có phải thread không?"

Bạn sẽ hiểu vì đã dùng `Task`.

### "Race condition là gì?"

Bạn có thể chỉ ngay vào Teacher Salary.

### "Làm sao xử lý background job?"

Bạn có `BackgroundService`.

### "Producer/Consumer pattern là gì?"

Bạn có `Channel<T>`.

### "Làm sao shutdown worker an toàn?"

Bạn có `CancellationToken`.

### "Nếu application crash khi đang ghi file?"

Bạn có backup/atomic persistence để thảo luận.

### "Làm sao tách business logic khỏi database?"

Bạn có:

```text
IRepository
    ↓
JsonRepository
```

---

# Mình đề xuất thứ tự học

Đừng làm toàn bộ V4 rồi toàn bộ V5 một cách mù mờ. Làm theo progression này:

```text
V4
 │
 ├── 4.1 System.Text.Json
 │
 ├── 4.2 File I/O
 │
 ├── 4.3 Generic Repository
 │
 ├── 4.4 JsonRepository
 │
 ├── 4.5 Load / Save
 │
 ├── 4.6 Error handling
 │
 └── 4.7 Backup / Recovery
 │
 ▼
V5
 │
 ├── 5.1 Task
 │
 ├── 5.2 async / await
 │
 ├── 5.3 Task.WhenAll
 │
 ├── 5.4 Concurrent students
 │
 ├── 5.5 Race condition
 │
 ├── 5.6 lock / thread safety
 │
 ├── 5.7 Channel<T>
 │
 ├── 5.8 BackgroundService
 │
 ├── 5.9 CancellationToken
 │
 └── 5.10 Graceful shutdown
```

**Đặc biệt, mình sẽ chưa đưa database vào.** JSON persistence + concurrency + worker sẽ dạy bạn nhiều kiến thức mới hơn việc nhảy ngay sang EF Core.

Và sau khi hoàn thành V5, mình nghĩ bước rất đẹp tiếp theo cho mục tiêu **.NET Developer** của bạn sẽ là:

```text
School Console
      │
      │ V1-V3
      ↓
OOP + Design Patterns
      │
      │ V4
      ↓
Persistence
      │
      │ V5
      ↓
Async + Concurrency + Workers
      │
      │ V6
      ↓
ASP.NET Core Web API
      │
      ↓
EF Core + SQL
      │
      ↓
Authentication / JWT
      │
      ↓
Testing
      │
      ↓
Docker
```

Tức là **không bỏ project School Simulator khi học ASP.NET Core**. Ta có thể giữ nguyên domain/business logic và "bọc" nó bằng Web API. Khi đó bạn sẽ thực sự thấy tại sao DI, Repository, Service, interface, separation of concerns... mà mình đã xây ở Console lại có ích trong ASP.NET Core.
