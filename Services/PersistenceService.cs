namespace School_CRUD_console.Services;

using School_CRUD_console.Interfaces;
using School_CRUD_console.Repository;
using School_CRUD_console.Models;
using System.Text.Json;

public class PersistenceService
{
    private readonly string _dataDir = "data";
    private readonly string _backupDir = "data/backup";
    private readonly ILogger _logger;

    public PersistenceService(ILogger logger)
    {
        _logger = logger;
    }

    public async Task SaveAllAsync(StudentRepository studentRepo, TeacherRepository teacherRepo)
    {
        await _logger.LogWarning("\n=== ĐANG THỰC HIỆN LƯU DỮ LIỆU & SAO LƯU (BACKUP) ===");

        await BackupFileIfExists("students.json");
        await BackupFileIfExists("teachers.json");

        Directory.CreateDirectory(_dataDir);
        Directory.CreateDirectory(_backupDir);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string studentPath = Path.Combine(_dataDir, "students.json");
        var students = await studentRepo.GetAllAsync();
        string studentJson = JsonSerializer.Serialize(students, options);
        await File.WriteAllTextAsync(studentPath, studentJson);
        await _logger.LogSuccess($"[LƯU THÀNH CÔNG] Đã lưu {students.Count} học sinh vào {studentPath}");

        string teacherPath = Path.Combine(_dataDir, "teachers.json");
        var teachers = await teacherRepo.GetAllAsync();
        string teacherJson = JsonSerializer.Serialize(teachers, options);
        await File.WriteAllTextAsync(teacherPath, teacherJson);
        await _logger.LogSuccess($"[LƯU THÀNH CÔNG] Đã lưu {teachers.Count} giáo viên vào {teacherPath}");
    }

    public async Task LoadAllAsync(StudentRepository studentRepo, TeacherRepository teacherRepo)
    {
        await _logger.LogWarning("\n=== ĐANG KHÔI PHỤC DỮ LIỆU TỪ ĐĨA CỨNG (JSON) ===");

        string studentPath = Path.Combine(_dataDir, "students.json");
        string teacherPath = Path.Combine(_dataDir, "teachers.json");

        if (File.Exists(studentPath))
        {
            try
            {
                string json = await File.ReadAllTextAsync(studentPath);

                var students = JsonSerializer.Deserialize<List<Student>>(json);

                if (students != null)
                {
                    foreach (var s in students) await studentRepo.AddAsync(s);

                    await _logger.LogSuccess($"[RECOVERY] Đã khôi phục {students.Count} học sinh từ file.");
                }
            }
            catch (JsonException ex)
            {
                await _logger.LogError($"[LỖI FILE JSON HỎNG] Không thể đọc {studentPath}: {ex.Message}");
            }
        }

        if (File.Exists(teacherPath))
        {
            try
            {
                string json = await File.ReadAllTextAsync(teacherPath);

                var teachers = JsonSerializer.Deserialize<List<Teacher>>(json);

                if (teachers != null)
                {
                    foreach (var t in teachers)
                    {
                        await teacherRepo.AddAsync(t);
                    }
                    await _logger.LogSuccess($"[RECOVERY] Đã khôi phục {teachers.Count} giáo viên từ file.");
                }
            }

            catch (JsonException ex)
            {
                await _logger.LogError($"[LỖI FILE JSON HỎNG] Không thể đọc {teacherPath}: {ex.Message}");

            }
        }
    }

    public async Task BackupFileIfExists(string fileName)
    {
        string sourceFile = Path.Combine(_dataDir, fileName);

        if (File.Exists(sourceFile))
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd__HHmmss");
            string nameWithOutExt = Path.GetFileNameWithoutExtension(fileName);

            string backupFileName = $"{nameWithOutExt}_{timestamp}.json";

            string destFile = Path.Combine(_backupDir, backupFileName);

            File.Copy(sourceFile, destFile, overwrite: true);

            await _logger.LogWarning($"[BACKUP] Đã sao lưu {fileName} -> backup/{backupFileName}");
        }

    }
}