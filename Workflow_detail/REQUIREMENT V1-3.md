# School Simulator v1

## Mục tiêu

Xây dựng một ứng dụng Console mô phỏng hoạt động của một trường học.

Hệ thống sẽ mô phỏng:

* Giáo viên
* Học sinh
* Môn học
* Bài kiểm tra
* Điểm số
* GPA
* Tiền lương giáo viên
* Nhật ký hoạt động (log)

Toàn bộ đều chạy bằng mô phỏng (random).

---

# Functional Requirement

## FR01 - Quản lý giáo viên

Mỗi giáo viên gồm

```
Id
Name
Age
Salary
Subject
ExperienceYears
```

Giáo viên có khả năng

```
CreateExam()
Teach()
EvaluateStudents()
```

---

## FR02 - Quản lý học sinh

Mỗi học sinh

```
Id
Name
Age
Grade
GPA
```

Có các hành động

```
TakeExam()
ReceiveScore()
CalculateGPA()
```

---

## FR03 - Môn học

Ví dụ

```
Math
Physics
Chemistry
English
History
```

Mỗi giáo viên chỉ dạy một môn.

---

## FR04 - Sinh đề thi

Đầu mỗi vòng

Giáo viên sẽ tạo một đề.

Ví dụ

```
Teacher Alice created Math Exam.

Difficulty : Hard
Question Count : 20
```

Độ khó được random

```
Easy
Medium
Hard
```

---

## FR05 - Học sinh làm bài

Mỗi học sinh

```
TakeExam()
```

được random điểm

Ví dụ

```
Difficulty Easy

80-100

Difficulty Medium

50-90

Difficulty Hard

20-80
```

Điểm lưu vào ExamResult.

---

## FR06 - GPA

Sau khi có điểm

Student sẽ

```
CalculateGPA()
```

Ví dụ

```
Math

90

Physics

80

English

100

Average

90

GPA = 3.8
```

---

## FR07 - Đánh giá giáo viên

Sau khi tất cả học sinh hoàn thành.

Teacher sẽ được đánh giá bằng

```
Average GPA
```

Ví dụ

```
Average GPA = 3.9

Salary += 500
```

Nếu

```
Average GPA < 2.5
```

thì

```
Salary -= 1000
```

Nếu lương nhỏ hơn

```
Threshold

5000
```

Giáo viên nghỉ việc.

---

## FR08 - Tuyển giáo viên mới

Nếu giáo viên nghỉ việc.

School sẽ

```
HireNewTeacher()
```

Random

```
Name

Age

Experience

Salary
```

Console

```
Teacher Alice resigned.

Hiring new teacher...

Teacher David joined school.
```

---

## FR09 - Chạy theo Semester

Một semester gồm

```
Teaching

↓

Create Exam

↓

Students Take Exam

↓

Calculate GPA

↓

Evaluate Teacher

↓

Generate Report
```

---

## FR10 - Nhật ký

Mọi hành động phải ghi log.

Ví dụ

```
2026-08-01 09:10

Teacher Alice created exam.
```

```
2026-08-01 09:15

Student Bob scored 89.
```

```
2026-08-01 09:20

Teacher Alice salary increased.
```

---

# Non Functional Requirement

## NFR01

Không dùng Database.

Toàn bộ dữ liệu lưu trong Memory.

---

## NFR02

Toàn bộ log ghi ra file.

Ví dụ

```
logs/

20260801.log
```

---

## NFR03

Console phải hiển thị màu.

Ví dụ

```
Green

Success

Yellow

Warning

Red

Error
```

---

## NFR04

Không dùng package ngoài.

Chỉ dùng .NET.

---

# Design

```
School
 │
 ├── Teachers
 │
 ├── Students
 │
 ├── Subjects
 │
 └── Semester
```

---

# Các class dự kiến

```
Person (abstract)

Teacher

Student

School

Exam

ExamResult

Subject

Semester

SalaryPolicy

GPACalculator

RandomGenerator

Notification

Logger
```

---

# Interface

```
ITeachable

IExamCreator

IExamTaker

ILogger

INotification

ISalaryPolicy

IGPACalculator
```

---

# Enum

```
SubjectType

ExamDifficulty

LogLevel

TeacherStatus
```

---

# Áp dụng OOP

### Encapsulation

```
private salary

public Salary
```

---

### Inheritance

```
Person

↓

Teacher

Student
```

---

### Polymorphism

```
Person

↓

Introduce()

Teacher

Student
```

---

### Abstraction

```
abstract Person
```

---

# Design Pattern nên áp dụng

Đây mới là phần thú vị nhất. Thay vì cố nhồi thật nhiều pattern, mình đề xuất mỗi pattern giải quyết đúng một vấn đề của hệ thống.

| Pattern                      | Áp dụng                                               | Ví dụ                                                            |
| ---------------------------- | ----------------------------------------------------- | ---------------------------------------------------------------- |
| **Repository**               | Quản lý dữ liệu trong bộ nhớ                          | `StudentRepository`, `TeacherRepository`                         |
| **Factory Method**           | Tạo Teacher, Student, Exam                            | `TeacherFactory`, `ExamFactory`                                  |
| **Strategy**                 | Tính lương, tính GPA                                  | `ISalaryPolicy`, `IGPACalculator`                                |
| **Observer**                 | Thông báo khi có sự kiện                              | Khi thi xong, khi giáo viên nghỉ việc, khi GPA thay đổi          |
| **Command**                  | Mỗi hành động là một command                          | `CreateExamCommand`, `TakeExamCommand`, `EvaluateTeacherCommand` |
| **Singleton** *(chỉ để học)* | Logger                                                | Một logger dùng chung toàn hệ thống                              |
| **Dependency Injection**     | Inject Logger, Notification, SalaryPolicy vào Service | Chuẩn bị cho ASP.NET Core sau này                                |

---

# Logging System

Thay vì chỉ ghi `.txt`, mình gợi ý thiết kế theo hướng có thể thay đổi "đầu ra" của log bằng Strategy:

```
ILogger
│
├── TextFileLogger
├── JsonLogger
├── CsvLogger
└── ConsoleLogger
```

Sau đó tạo một `CompositeLogger`:

```
CompositeLogger

↓

ConsoleLogger

↓

TextLogger

↓

JsonLogger
```

Mỗi khi gọi:

```csharp
logger.Log("Teacher created exam");
```

thì đồng thời:

* Hiện lên Console.
* Ghi vào `logs/20260802.txt`.
* Ghi thêm vào `logs/20260802.json`.
* (Tuỳ chọn) Ghi CSV để dễ mở bằng Excel.

Đây là một ví dụ rất thực tế về **Composite Pattern**, đồng thời tận dụng **Dependency Injection** vì các logger đều được truyền qua interface.

---

# Notification System

Để mô phỏng "worker" hoặc một tiến trình khác trong tương lai, mình sẽ không cho notification chỉ in ra màn hình. Thay vào đó:

```
INotificationChannel
│
├── ConsoleNotification
├── FileNotification
└── NotificationQueue
```

Khi hệ thống phát sinh sự kiện:

```
Teacher Alice created Exam.
```

Notification sẽ:

1. Hiện lên Console.
2. Ghi một file vào thư mục `notifications/`.
3. (Sau này) Có thể thay `FileNotification` bằng RabbitMQ, Kafka hay SignalR mà không phải sửa business logic.

Điều này giúp bạn làm quen sớm với kiến trúc **event-driven** mà vẫn chỉ dùng file để mô phỏng.

---

# Roadmap phát triển


* **v1 – OOP Foundation:** `Person`, `Student`, `Teacher`, CRUD cơ bản, kế thừa, interface.
* **v2 – Simulation Engine:** Semester, tạo đề, làm bài, GPA, lương giáo viên.
* **v3 – Design Patterns:** Factory, Strategy, Repository, Observer, Composite Logger.
* **v4 – Persistence:** Lưu/đọc JSON, CSV, TXT, khôi phục trạng thái khi khởi động.
* **v5 – Concurrency:** Dùng `Task` và `async/await` để mô phỏng nhiều học sinh làm bài cùng lúc và logger/notification hoạt động như các worker nền.
* **v6 – ASP.NET Core Migration:** Giữ nguyên business logic, chỉ thay Console UI bằng REST API, tận dụng DI của ASP.NET Core.

