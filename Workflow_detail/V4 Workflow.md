Edited Teacher.cs
Edited StudentFactory.cs
Edited Program.cs
Edited TeacherFactory.cs
Edited Semester.cs
Edited Semester.cs
Edited Student.cs
Edited School.cs
Edited REQUIREMENT%20V4-5.md
Viewed exam.json

Chúc mừng bạn đã hoàn thành xuất sắc **Version 3**! 

Nâng cấp lên **Version 4 (Persistence & Recovery)** là bước chuyển mình quan trọng nhất để biến ứng dụng của bạn từ một bài tập OOP trên bộ nhớ RAM thành một **Hệ thống phần mềm thực thụ có khả năng lưu trữ & khôi phục dữ liệu**, rất có giá trị khi phỏng vấn lập trình viên .NET!

Dưới đây là hướng dẫn chi tiết về những gì bạn cần học, kiến trúc cần áp dụng và roadmap từng bước triển khai V4 theo lựa chọn **Option C (PersistenceService + Backup)**.

---

## 📚 1. Những kiến thức mới bạn cần học cho Version 4

| Chủ đề / Kỹ thuật | Khái niệm & Ứng dụng |
| :--- | :--- |
| **`System.Text.Json`** | Thư viện chuẩn của .NET để chuyển đổi Đối tượng C# $\leftrightarrow$ Chuỗi JSON.<br>• `JsonSerializer.SerializeAsync()` (Lưu thành JSON)<br>• `JsonSerializer.DeserializeAsync()` (Đọc từ JSON)<br>• `JsonSerializerOptions` (`WriteIndented = true` để format JSON đẹp). |
| **`System.IO` (File System)** | Quản lý File và Thư mục:<br>• `Directory.CreateDirectory("data/backup")`<br>• `File.Exists()`, `File.Copy()`, `File.WriteAllTextAsync()`, `Path.Combine()`. |
| **C# Generics (`<T>`)** | Viết code tái sử dụng cho nhiều lớp bằng kiểu đại diện `T`.<br>• Viết 1 Interface `IRepository<T>` dùng chung cho cả `Student`, `Teacher`, `Exam`. |
| **Exception Handling (Xử lý lỗi)** | Xử lý các sự cố thực tế khi thao tác file:<br>• `FileNotFoundException` (khi chưa có file data).<br>• `JsonException` (khi file JSON bị ai đó sửa hỏng cấu trúc). |

---

## 🏗️ 2. Kiến trúc Version 4 (Save Strategy Option C + Backup)

Với lựa chọn **Option C + Backup**, hệ thống của bạn sẽ được tổ chức theo sơ đồ sau:

```
                            ┌───────────────────────────┐
                            │    PersistenceService     │ (Tầng quản lý Lưu trữ & Backup)
                            └─────────────┬─────────────┘
                                          │
                  ┌───────────────────────┴───────────────────────┐
                  ▼                                               ▼
     ┌─────────────────────────┐                     ┌─────────────────────────┐
     │ JsonRepository<Student> │                     │ JsonRepository<Teacher> │
     └────────────┬────────────┘                     └────────────┬────────────┘
                  │                                               │
                  ▼                                               ▼
        data/students.json                              data/teachers.json
        data/backup/students_20260916.json              data/backup/teachers_20260916.json
```

---

## 🛠️ 3. Chi tiết 4 Thành phần mới cần xây dựng

### 🧩 Thành phần 1: Generic Repository Interface (`IRepository<T>`)
Thay vì viết các repo riêng lẻ không có chuẩn chung, bạn tạo một Interface Generic tổng quát:

```csharp
namespace School_CRUD_console.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IReadOnlyList<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task DeleteAsync(T entity);
    Task LoadAsync(string filePath);
    Task SaveAsync(string filePath);
}
```
> **Điểm phỏng vấn giá trị:** *"Business logic của ứng dụng chỉ phụ thuộc vào `IRepository<T>`. Sau này muốn đổi từ lưu JSON sang SQL Database, tôi chỉ cần tạo `SqlRepository<T>` mà không phải sửa logic của School hay Semester."* (Đó chính là nguyên lý **Dependency Inversion - DIP** trong SOLID).

---

### 🧩 Thành phần 2: Lớp `JsonRepository<T>`
Kế thừa từ `IRepository<T>`, giữ một `List<T>` trong bộ nhớ RAM để ứng dụng chạy nhanh, đồng thời cung cấp hàm đọc/ghi file `.json`:

- **Hàm `LoadAsync(filePath)`:** Đọc file JSON $\rightarrow$ giải mã bằng `JsonSerializer.DeserializeAsync` $\rightarrow$ nạp vào danh sách `List<T>`. Nếu file chưa tồn tại $\rightarrow$ tự tạo danh sách rỗng.
- **Hàm `SaveAsync(filePath)`:** Mã hóa `List<T>` thành chuỗi JSON $\rightarrow$ ghi xuống đĩa cứng bằng `JsonSerializer.SerializeAsync`.

---

### 🧩 Thành phần 3: Lớp `PersistenceService` (Save Strategy Option C + Backup)
Lớp này đóng vai trò như một **Trạm quản lý tập trung việc Lưu trữ & Sao lưu**:

#### 🔄 Cơ chế Sao lưu (Backup) trước khi Save:
1. Mỗi khi chuẩn bị ghi đè file mới vào `data/students.json`:
2. `PersistenceService` kiểm tra xem file `data/students.json` cũ có tồn tại không.
3. Nếu có, tiến hành **Copy file cũ** sang thư mục sao lưu:
   `data/backup/students_20260916_103000.json` (kèm ngày giờ hiện tại).
4. Sau khi backup an toàn xong mới tiến hành ghi file `data/students.json` mới!

#### 🔄 Cơ chế Application Recovery khi khởi động:
1. Khi ứng dụng vừa bật lên (`School.InitializeData()`):
2. `PersistenceService.LoadAllAsync()` sẽ kiểm tra thư mục `data/`.
3. Đọc dữ liệu từ `students.json`, `teachers.json` khôi phục lại trạng thái cũ của trường học.
4. **Bảo vệ chống Crash (Exception Handling):** Nếu file JSON bị hỏng (`JsonException`), `PersistenceService` sẽ tự động phát hiện, báo lỗi ra log và thử tải bản sao lưu gần nhất trong `data/backup/` để ứng dụng không bị chết đột ngột.

---

### 🧩 Thành phần 4: Tích hợp vào `School.cs` & `Program.cs`

Trong `Program.cs`, ứng dụng của bạn sẽ có luồng hoạt động chuẩn như sau:

```csharp
// 1. Khởi tạo Logger & PersistenceService
ILogger logger = new ConsoleLogger();
PersistenceService persistence = new PersistenceService(logger);

// 2. Khởi tạo Trường học & Tải dữ liệu cũ từ đĩa cứng (Recovery)
School school = new School(logger, persistence);
await school.LoadSavedDataAsync(); // Đọc dữ liệu cũ nếu có

// 3. Nếu là lần đầu chạy (chưa có dữ liệu), mới khởi tạo dữ liệu mẫu ngẫu nhiên
if (!school.HasData)
{
    school.InitializeDefaultData(20);
}

// 4. Chạy mô phỏng các Học kỳ
school.StartSimulation(2);

// 5. Kết thúc mô phỏng -> Tự động Sao lưu (Backup) và Lưu toàn bộ dữ liệu mới (Option C Save)
await school.SaveAllDataAsync();
```

---

## 🗺️ Roadmap các bước thực hiện tiếp theo

Để làm v4 một cách mạch lạc, mình đề xuất lộ trình làm theo từng bước nhỏ:

1. **Bước 4.1:** Tạo thư mục `data/` và thử nghiệm với `System.Text.Json` (Đọc/ghi thử 1 danh sách Student đơn giản ra file JSON).
2. **Bước 4.2:** Tạo Interface Generic `IRepository<T>`.
3. **Bước 4.3:** Xây dựng `JsonRepository<T>` thay thế cho các Repository cũ.
4. **Bước 4.4:** Xây dựng `PersistenceService` xử lý Backup + Save Option C.
5. **Bước 4.5:** Tích hợp Load/Save/Backup vào `School.cs` và kiểm tra khả năng khôi phục dữ liệu sau khi tắt app và bật lại.

Bạn cảm thấy lộ trình này thế nào? Nếu bạn sẵn sàng, chúng ta có thể bắt đầu từng bước một từ **Bước 4.1**!