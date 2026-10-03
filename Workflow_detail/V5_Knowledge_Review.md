# 📚 Tổng Hợp Kiến Thức Trọng Tâm Version 5: Concurrency, Event-Driven & Background Worker

> **Dự án:** School CRUD Console (School Simulator)  
> **Mục tiêu tài liệu:** Tổng hợp lại toàn bộ lý thuyết, khái niệm và ví dụ minh họa bằng C# của Version 5 để làm tài liệu học tập & chuẩn bị cho phỏng vấn .NET Developer.

---

## 📑 Mục Lục
1. [Lập Trình Bất Đồng Bộ (Async/Await & Concurrency)](#1-lập-trình-bất-đồng-bộ-asyncawait--concurrency)
2. [Kiến Trúc Hướng Sự Kiện (Event-Driven Architecture & Producer-Consumer)](#2-kiến-trúc-hướng-sự-kiện-event-driven-architecture--producer-consumer)
3. [Hàng Đợi Tin Nhắn Với `System.Threading.Channels`](#3-hàng-đợi-tin-nhắn-với-systemthreadingchannels)
4. [Xử Lý Đa Luồng Ngầm (Background Worker & Pattern Matching)](#4-xử-lý-đa-luồng-ngầm-background-worker--pattern-matching)
5. [An Toàn Đa Luồng & Xung Đột Dữ Liệu (Thread Safety & Race Condition)](#5-an-toàn-đa-luồng--xung-đột-dữ-liệu-thread-safety--race-condition)
6. [Tắt Ứng Dụng An Toàn (Graceful Shutdown & CancellationToken)](#6-tắt-ứng-dụng-an-toàn-graceful-shutdown--cancellationtoken)

---

## 1. Lập Trình Bất Đồng Bộ (Async/Await & Concurrency)

### 💡 Khái niệm cốt lõi
- **Async/Await**: Cho phép giải phóng Thread hiện tại trong lúc chờ các tác vụ tốn thời gian (I/O, Đọc/Ghi file, Gọi API, Query DB, `Task.Delay`). Thread giải phóng sẽ quay về **ThreadPool** để làm việc khác, giúp ứng dụng phản hồi mượt mà.
- **Concurrency (Đồng thời)** vs **Parallelism (Song song)**:
  - **Concurrency**: Các công việc chuyển đổi qua lại luân phiên trong một khoảng thời gian (không nhất thiết phải chạy trên nhiều nhân CPU cùng một thời điểm).
  - **Parallelism**: Các công việc chạy song song thực sự cùng một thời điểm trên nhiều nhân CPU (Multi-threading / Multi-core).
- **`IAsyncStateMachine`**: Khi biên dịch, C# compiler sẽ chuyển đổi các hàm có từ khóa `async` thành một **State Machine** để quản lý trạng thái tạm dừng (`await`) và khôi phục khi tác vụ hoàn thành.

### 💻 Ví dụ C# (Gom nhóm và chạy Task đồng thời với `Task.WhenAll`)

```csharp
// Giả lập 1 bài thi chạy bất đồng bộ
public async Task TakeExamAsync(Exam exam)
{
    // Task.Delay giải phóng thread trong lúc giả lập làm bài
    await Task.Delay(_rand.Next(100, 500)); 
    // Chấm điểm...
}

// Chạy tất cả sinh viên làm tất cả các bài thi ĐỒNG THỜI
List<Student> students = await _studentRepo.GetAllAsync();
List<Exam> globalExams = GetGlobalExams();

// Tạo danh sách Task bất đồng bộ (chưa cho await từng cái một)
IEnumerable<Task> examTasks = students.SelectMany(student =>
    globalExams.Select(exam => student.TakeExamAsync(exam))
);

// Task.WhenAll gom tất cả các Task lại và chờ TẤT CẢ hoàn thành đồng thời!
await Task.WhenAll(examTasks);
```

---

## 2. Kiến Trúc Hướng Sự Kiện (Event-Driven Architecture & Producer-Consumer)

### 💡 Khái niệm cốt lõi
Mô hình **Event-Driven** tách rời (decouple) nơi phát sinh sự kiện và nơi xử lý sự kiện:
- **Producer (Người phát sự kiện)**: Sinh ra tin nhắn sự kiện khi có hành động xảy ra (ví dụ: Học sinh làm xong bài thi, Giáo viên nghỉ việc) và đẩy vào Hàng chờ (Queue).
- **Message Queue (Hàng đợi tin nhắn)**: Bộ nhớ đệm trung gian lưu trữ các sự kiện theo cơ chế FIFO (First-In, First-Out).
- **Consumer (Người tiêu thụ)**: Tiến trình chạy ngầm liên tục rút sự kiện từ Queue ra để xử lý (Ghi log, gửi mail, lưu DB...).

### 🗺 Ánh xạ tới Hệ thống Thực tế (RabbitMQ / Apache Kafka)
| Khái niệm trong Dự án | RabbitMQ / Kafka Thực tế | Vai trò |
| :--- | :--- | :--- |
| **`SchoolEvent`** | **Message / Payload** | Chứa thông tin sự kiện dạng Object |
| **`Student` / `Semester`** | **Producer / Publisher** | Đẩy tin nhắn vào Queue |
| **`SchoolEventQueue`** | **Queue / Topic / Exchange** | Nơi chứa tin nhắn trung gian |
| **`SchoolEventWorker`** | **Consumer / Subscriber** | Tiến trình chạy ngầm đọc và xử lý |

### 💻 Ví dụ C# (Định nghĩa Event Polymorphism)

```csharp
// Abstract Event base class
public abstract class SchoolEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime Timestamp { get; } = DateTime.Now;
    public abstract string Message { get; }
}

// Sự kiện cụ thể
public class ExamCompleteEvent : SchoolEvent
{
    public string StudentName { get; }
    public Subject Subject { get; }
    public int Grade { get; }

    public ExamCompleteEvent(string studentName, Subject subject, int grade)
    {
        (StudentName, Subject, Grade) = (studentName, subject, grade);
    }

    public override string Message => 
        $"[EXAM COMPLETE] Sinh viên {StudentName} thi môn {Subject} - Điểm: {Grade}";
}
```

---

## 3. Hàng Đợi Tin Nhắn Với `System.Threading.Channels`

### 💡 Khái niệm cốt lõi
- **`System.Threading.Channels`**: Là thư viện hàng đợi Message Queue nội bộ (In-memory) hiệu năng cao nhất trong .NET, chuyên dùng cho bài toán **Producer-Consumer**.
- Hỗ trợ đầy đủ bất đồng bộ (`ValueTask`, `IAsyncEnumerable<T>`) và an toàn đa luồng (Thread-safe).
- `Channel.CreateUnbounded<T>()`: Tạo hàng đợi không giới hạn dung lượng.

### 💻 Ví dụ C# (Triển khai `SchoolEventQueue`)

```csharp
using System.Threading.Channels;

public class SchoolEventQueue
{
    private readonly Channel<SchoolEvent> _channel = Channel.CreateUnbounded<SchoolEvent>(
        new UnboundedChannelOptions { SingleReader = true }
    );

    // Producer đẩy tin nhắn vào Queue
    public async ValueTask PublishAsync(SchoolEvent schoolEvent)
    {
        await _channel.Writer.WriteAsync(schoolEvent);
    }

    // Đóng chiều ghi tin nhắn (báo hiệu Producer đã xong)
    public void Complete()
    {
        _channel.Writer.Complete();
    }

    // Consumer đọc tin nhắn dưới dạng luồng bất đồng bộ IAsyncEnumerable
    public IAsyncEnumerable<SchoolEvent> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
```

---

## 4. Xử Lý Đa Luồng Ngầm (Background Worker & Pattern Matching)

### 💡 Khái niệm cốt lõi
- **Background Worker**: Tiến trình chạy song song ở luồng nền (background task) độc lập với luồng chính của ứng dụng.
- **`IAsyncEnumerable<T>` & `await foreach`**: Cú pháp duyệt danh sách bất đồng bộ theo luồng (streaming data). Vòng lặp sẽ tạm treo dừng chờ khi Queue trống và tự động chạy tiếp khi có item mới.
- **Pattern Matching (`switch-case` trên Type)**: Giúp kiểm tra kiểu dữ liệu của Event một cách sạch sẽ và linh hoạt.

### 💻 Ví dụ C# (Triển khai Consumer `SchoolEventWorker`)

```csharp
public class SchoolEventWorker
{
    private readonly SchoolEventQueue _queue;
    private readonly ILogger _logger;

    public SchoolEventWorker(SchoolEventQueue queue, ILogger logger)
    {
        (_queue, _logger) = (queue, logger);
    }

    public async Task StartProcessingAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // await foreach tự động duyệt qua IAsyncEnumerable của Channel
            await foreach (var schoolEvent in _queue.ReadAllAsync(cancellationToken))
            {
                await ProcessEventAsync(schoolEvent);
            }
        }
        catch (OperationCanceledException)
        {
            await _logger.LogWarning("[WORKER] Nhận tín hiệu dừng...");
        }
    }

    private async Task ProcessEventAsync(SchoolEvent evt)
    {
        // Pattern Matching theo kiểu Event
        switch (evt)
        {
            case ExamCompleteEvent examEvt:
                await _logger.LogInfo(examEvt.Message);
                break;
            case TeacherResignedEvent resignedEvt:
                await _logger.LogError(resignedEvt.Message);
                break;
            default:
                await _logger.LogInfo(evt.Message);
                break;
        }
    }
}
```

---

## 5. An Toàn Đa Luồng & Xung Đột Dữ Liệu (Thread Safety & Race Condition)

### 💡 Khái niệm cốt lõi
- **Race Condition (Xung đột dữ liệu)**: Xảy ra khi nhiều luồng/Task cùng đồng thời đọc và ghi vào cùng một vùng nhớ dùng chung (**Shared State**) mà không được đồng bộ.
- **Tại sao `List<T>` không Thread-Safe?**: `List<T>` dùng một mảng nội bộ. Khi 2 luồng cùng gọi `.Add()` một lúc, cả 2 sẽ đọc cùng chỉ số mảng và ghi đè lên nhau, gây ra mất dữ liệu hoặc ném ngoại lệ `IndexOutOfRangeException`.
- **Cơ chế `lock` (Mutual Exclusion - Mutex)**: Đảm bảo tại một thời điểm chỉ có **ĐÚNG 1 LUỒNG** được phép đi vào vùng code được bảo vệ (Critical Section).

### 💻 Ví dụ C# (Khử Race Condition bằng `lock`)

```csharp
public class Student : Person
{
    private readonly List<ExamResult> _examResults = new();
    
    // Khai báo một object riêng biệt làm chìa khóa lock
    private readonly object _examResultLock = new object();

    public async Task TakeExamAsync(Exam exam, SchoolEventQueue queue)
    {
        await Task.Delay(_rand.Next(100, 500));
        int grade = CalculateGrade(exam);

        // Đảm bảo an toàn đa luồng khi Add vào List dùng chung
        lock (_examResultLock)
        {
            _examResults.Add(new ExamResult { exam = exam, Grade = grade });
        }

        // Đẩy event vào Queue
        await queue.PublishAsync(new ExamCompleteEvent(Name, exam.Subject, grade));
    }
}
```

---

## 6. Tắt Ứng Dụng An Toàn (Graceful Shutdown & CancellationToken)

### 💡 Khái niệm cốt lõi
- **`CancellationTokenSource` & `CancellationToken`**: Cơ chế báo hiệu hủy tác vụ bất đồng bộ chuẩn trong .NET.
- **Graceful Shutdown (Tắt ứng dụng an toàn)**: Đảm bảo trước khi ứng dụng đóng hoàn toàn:
  1. Không nhận thêm công việc mới (`queue.Complete()`).
  2. Xử lý sạch sẽ 100% các tin nhắn dở dang còn lại trong Queue (**Zero Message Loss**).
  3. Đợi Worker dừng hoàn toàn (`await workerTask`) rồi mới lưu dữ liệu xuống đĩa.

### 💻 Ví dụ C# (Lắp ráp hoàn chỉnh trong `Program.cs`)

```csharp
// 1. Khởi tạo Queue & Background Worker
SchoolEventQueue queue = new SchoolEventQueue();
SchoolEventWorker worker = new SchoolEventWorker(queue, logger);

using CancellationTokenSource cts = new CancellationTokenSource();

// 2. Chạy Worker ngầm ở luồng nền (Task.Run)
Task workerTask = Task.Run(() => worker.StartProcessingAsync(cts.Token));

// 3. Thực thi Simulation chính của ứng dụng
School school = new School(logger, gpaCalc, persistence, queue);
await school.StartSimulationAsync(2);

// 4. KÍCH HOẠT GRACEFUL SHUTDOWN:
// Bước 4.1: Đóng chiều ghi tin nhắn (Không cho gửi tin nhắn mới)
queue.Complete();

// Bước 4.2: Chờ Worker rút hết tin nhắn tồn đọng trong Queue và tự ngắt
await workerTask;

// Bước 4.3: Hủy CancellationToken và lưu toàn bộ dữ liệu an toàn
cts.Cancel();
await school.SaveAllDataAsync();
```

---

## 🎯 Tổng Kết Ngắn Gọn Cho Phỏng Vấn (Elevator Pitch)

> *"Trong dự án School Simulator, em đã nâng cấp hệ thống từ xử lý tuần tự sang **Kiến trúc Hướng sự kiện (Event-Driven Architecture)**. Em sử dụng `Task.WhenAll` và `Task.Delay` để giả lập quá trình sinh viên làm bài thi đồng thời. Khi sinh viên nộp bài, hệ thống đóng vai **Producer** phát sự kiện vào hàng đợi `System.Threading.Channels`. Ở phía dưới, một **Background Worker** chạy bất đồng bộ đóng vai **Consumer** rút sự kiện ra để ghi log và xử lý ngầm. Để đảm bảo tính toàn vẹn dữ liệu khi nhiều luồng ghi điểm cùng lúc, em áp dụng cơ chế `lock` tránh **Race Condition**. Cuối cùng, em triển khai **Graceful Shutdown** với `queue.Complete()` và `CancellationToken` giúp đảm bảo xử lý hết 100% tin nhắn tồn đọng trước khi đóng ứng dụng."*
