using System.Diagnostics;

class Program
{
    static void Main()
    {
        Console.WriteLine(@"MinecraftUnlocker от A-Generation
Данная программа выполняет следующие действия:
Получает права на файлы Windows.ApplicationModel.Store.dll в папках System32 и SysWOW64.
Удаляет их.
Восстанавливает файлы из копий, находящихся в папке программы.
Использование PowerShell
Программа использует внешний PowerShell для выполнения команд с повышенными правами. При запуске PowerShell может открывать дополнительные консольные окна, бояться их не стоит – это нормальный процесс работы.

Важные рекомендации перед запуском:
Сделайте резервную копию файлов перед использованием программы, чтобы можно было их восстановить в случае непредвиденных проблем.
Перед запуском распакуйте архив в отдельную папку. Программа использует внешние файлы для восстановления (system32_Windows.ApplicationModel.Store.dll и syswow64_Windows.ApplicationModel.Store.dll), и если она будет запущена прямо из архива, доступ к этим файлам будет заблокирован.

Запуск:
Распакуйте архив в отдельную папку.
При запросе доступа администратора всегда разрешайте во избежание поломок в системе (или сразу запустите программу от имени администратора).
Следуйте инструкциям на экране.

Примечание
После выполнения всех операций программа отобразит соответствующие сообщения о статусе выполнения команд.
Если какие-то процессы использовали удаляемые файлы, программа автоматически получит к ним доступ.

Программа предназначена для восстановления лицензии для запуска Minecraft Bedrock Edition и работает только на Windows.
Используйте на свой страх и риск. Разработчик не несет ответственности за возможные последствия.
");
        Console.WriteLine("\nВы уверены, что хотите выполнить эти действия? (да/нет)");

        string userInput = Console.ReadLine();
        if (userInput?.ToLower() != "да")
        {
            Console.WriteLine("Операция отменена.");
            return;
        }

        string[] files =
        {
            @"C:\Windows\System32\Windows.ApplicationModel.Store.dll",
            @"C:\Windows\SysWOW64\Windows.ApplicationModel.Store.dll"
        };

        // Получаем права на файлы
        foreach (string file in files)
        {
            GrantPermissions(file);
        }

        Console.WriteLine("\nНажмите Enter для удаления файлов...");
        Console.ReadLine();

        // Удаляем файлы
        foreach (string file in files)
        {
            DeleteFile(file);
        }

        Console.WriteLine("\nФайлы удалены. Нажмите Enter для восстановления...");
        Console.ReadLine();

        // Восстанавливаем файлы
        RestoreFiles();

        Console.WriteLine("\nОперация завершена! Нажмите Enter для выхода.");
        Console.ReadLine();
    }

    // Функция для получения прав на файл
    static void GrantPermissions(string filePath)
    {
        string command = $"takeown /f \"{filePath}\" && icacls \"{filePath}\" /grant *S-1-3-4:F /t /c /l";
        if (RunCommandAsAdmin(command))
        {
            Console.WriteLine($"Права на файл {filePath} успешно получены.");
        }
        else
        {
            Console.WriteLine($"Ошибка при получении прав на файл {filePath}.");
        }
    }

    // Функция для удаления файла
    static void DeleteFile(string filePath)
    {
        string command = $"del /f /q \"{filePath}\"";
        if (RunCommandAsAdmin(command))
        {
            Console.WriteLine($"Файл {filePath} успешно удален.");
        }
        else
        {
            Console.WriteLine($"Ошибка при удалении файла {filePath}.");
        }
    }

    // Функция для восстановления файлов
    static void RestoreFiles()
    {
        string programFolder = AppDomain.CurrentDomain.BaseDirectory; // Папка, где находится программа

        string sourceFile1 = Path.Combine(programFolder, "system32_Windows.ApplicationModel.Store.dll");
        string sourceFile2 = Path.Combine(programFolder, "syswow64_Windows.ApplicationModel.Store.dll");

        string destinationFile1 = @"C:\Windows\System32\Windows.ApplicationModel.Store.dll";
        string destinationFile2 = @"C:\Windows\SysWOW64\Windows.ApplicationModel.Store.dll";

        CopyFileWithAdminRights(sourceFile1, destinationFile1);
        CopyFileWithAdminRights(sourceFile2, destinationFile2);
    }

    // Функция для копирования файлов с правами администратора
    static void CopyFileWithAdminRights(string source, string destination)
    {
        if (File.Exists(source))
        {
            string command = $"copy /Y \"{source}\" \"{destination}\"";
            if (RunCommandAsAdmin(command))
            {
                Console.WriteLine($"Файл {source} успешно восстановлен в {destination}.");
            }
            else
            {
                Console.WriteLine($"Ошибка при восстановлении файла {source} в {destination}.");
            }
        }
        else
        {
            Console.WriteLine($"Файл {source} не найден! Пропускаем...");
        }
    }

    // Универсальная функция для выполнения команд с правами администратора
    static bool RunCommandAsAdmin(string command)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "powershell",
            Arguments = $"-WindowStyle Hidden -Command \"Start-Process cmd -ArgumentList '/c {command}' -Verb runAs\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            Verb = "runas"
        };

        try
        {
            Process process = Process.Start(psi);

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                Console.WriteLine("Команда выполнена успешно.");
                return true;
            }
            else
            {
                Console.WriteLine($"Ошибка: {error}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при выполнении команды: {ex.Message}");
            return false;
        }
    }
}
