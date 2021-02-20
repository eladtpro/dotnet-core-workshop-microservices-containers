using Microsoft.WindowsAzure.Storage.Auth;

namespace AdminTools.TableStorage
{
    //TODO: check if needed
    internal class CloudStorageAccount
    {
        private StorageCredentials storageCredentials;
        private bool v;

        public CloudStorageAccount(StorageCredentials storageCredentials, bool v)
        {
            this.storageCredentials = storageCredentials;
            this.v = v;
        }
    }
}