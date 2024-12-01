using System.Net.Mime;
using System.Text;
using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.DownloadJobFile.Data;

internal static class TestCase03
{
    public static TestCaseData Get()
    {
        // Return existing file.
        return new TestCaseData
        {
            TestCaseId = 3,
            JobId = "21CB9FB1-8FE9-45B9-89CB-45BAAF90CF5E",
            Data = new BaseTestCaseData
            {
                StorageAccount = new StorageAccountTestCaseData
                {
                    OutputFiles =
                    [
                        new JobFileInfo
                        {
                            JobId = Guid.Parse("21CB9FB1-8FE9-45B9-89CB-45BAAF90CF5E"),
                            File = new JobFile
                            {
                                FileName = "test-file.json",
                                ContentType = MediaTypeNames.Application.Json,
                                Content = new MemoryStream(
                                    Encoding.UTF8.GetBytes("{\"key1\": \"value1\", \"key2\": \"value2\"}")
                                )
                            }
                        }
                    ]
                }
            }
        };
    }
}
