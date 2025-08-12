using LokPass.Core.Password.Crypto;

namespace LokPass.Core.TestData;
public static class TestDataService
{
    /// <summary>
    /// Creates a test user configuration with fake values for development.
    /// </summary>
    public static UserConfiguration CreateTestUserConfiguration()
    {
        var testMasterKey = "test-master-key-32-chars-long!!"u8.ToArray();
        var testSalt = "test-salt-16char"u8.ToArray();

        if (testMasterKey.Length != 32)
        {
            Array.Resize(ref testMasterKey, 32);
        }
        
        if (testSalt.Length != 16)
        {
            Array.Resize(ref testSalt, 16);
        }

        return new UserConfiguration(testMasterKey, testSalt);
    }
}
