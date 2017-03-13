using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using YoutubeQueuer.Lib.Providers;
using NUnit.Framework;
using Xunit;
using Assert = NUnit.Framework.Assert;

namespace YoutubeQueuer.Tests.Providers
{

    [TestFixture]
    public class FileSystemPersistenceProviderTests : IDisposable
    {
        private FileSystemPersistenceProvider _target;
        private const string TestFileName = "test_file_name.json";

        public FileSystemPersistenceProviderTests()
        {
            Setup();
        }

        [SetUp]
        public void Setup()
        {
            _target = new FileSystemPersistenceProvider();
        }

        [Fact]
        [Test]
        public void PersistData_ForSimpleType_ShouldSucceed()
        {
            // Arrange
            const string data = "this_is_a_test_string";

            // Act
            _target.PersistData(data, TestFileName);

            // Assert
            var fullPath = GetFullPath(TestFileName);
            FileAssert.Exists(fullPath);
        }

        [Fact]
        [Test]
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

            Assert.AreEqual(data, converted);
        }

        [Fact]
        [Test]
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
            FileAssert.Exists(fullPath);
        }

        [Fact]
        [Test]
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

            CollectionAssert.AreEquivalent(data, converted);
        }

        [Fact]
        [Test]
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
            CollectionAssert.AreEquivalent(data, actual);
        }
        
        [TearDown]
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
            var basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            var combinedPath = Path.Combine(basePath, fileName);
            return combinedPath;
        }

        public void Dispose()
        {
            CleanUp();
        }
    }
}
