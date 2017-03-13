using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using YoutubeQueuer.Lib.Providers;
using Xunit;

namespace YoutubeQueuer.Tests.Providers
{
    public class FileSystemPersistenceProviderTests : IDisposable
    {
        private FileSystemPersistenceProvider _target;
        private const string TestFileName = "test_file_name.json";

        public FileSystemPersistenceProviderTests()
        {
            Setup();
        }

        public void Setup()
        {
            _target = new FileSystemPersistenceProvider();
        }

        [Fact]
        public void PersistData_ForSimpleType_ShouldSucceed()
        {
            // Arrange
            const string data = "this_is_a_test_string";

            // Act
            _target.PersistData(data, TestFileName);

            // Assert
            var fullPath = GetFullPath(TestFileName);
            Assert.True(File.Exists(fullPath));
        }

        [Fact]
        public void PersistData_ForSimpleType_ShouldContainCorrectData()
        {
            // Arrange
            const string data = "this_is_a_test_string";

            // Act
            _target.PersistData(data, TestFileName);

            // Assert
            var fullPath = GetFullPath(TestFileName);
            var file = File.ReadAllText(fullPath);
            var converted = JsonConvert.DeserializeObject<string>(file);

            Assert.Equal(data, converted);
        }

        [Fact]
        public void PersistData_ForCollection_ShouldSucceed()
        {
            // Arrange
            var data = new List<int>
            {
                12,
                60,
                1337
            };

            // Act
            _target.PersistData(data, TestFileName);
            var fullPath = GetFullPath(TestFileName);

            // Assert
            Assert.True(File.Exists(fullPath));
        }

        [Fact]
        public void PersistData_ForCollection_ShouldContainCorrectData()
        {
            // Arrange
            var data = new List<int>
            {
                12,
                60,
                1337
            };

            // Act
            _target.PersistData(data, TestFileName);

            // Assert
            var fullPath = GetFullPath(TestFileName);
            var file = File.ReadAllText(fullPath);
            var converted = JsonConvert.DeserializeObject<IEnumerable<int>>(file);

            Assert.Equal(data, converted);
        }

        [Fact]
        public void GetData_ForValidFile_ShouldReturnContainedData()
        {
            // Arrange
            var data = new List<long>
            {
                2, -1, 17, 43
            };
            var serialized = JsonConvert.SerializeObject(data);
            var fullPath = GetFullPath(TestFileName);
            File.WriteAllText(fullPath, serialized);

            // Act
            var actual = _target.GetDataOrDefault<IEnumerable<long>>(TestFileName);

            // Assert
            Assert.Equal(data, actual);
        }
        
        public void CleanUp()
        {
            var fullPath = GetFullPath(TestFileName);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        private static string GetFullPath(string fileName)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var combinedPath = Path.Combine(basePath, fileName);
            return combinedPath;
        }

        public void Dispose()
        {
            CleanUp();
        }
    }
}
