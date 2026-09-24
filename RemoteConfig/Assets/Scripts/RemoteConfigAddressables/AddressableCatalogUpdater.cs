using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FirebaseExamples.RemoteConfigAddressables
{
    public sealed class AddressableCatalogUpdater : MonoBehaviour
    {
        public async Task UpdateAsync()
        {
            var check = Addressables.CheckForCatalogUpdates(false);

            try
            {
                await check.Task;

                if (this == null)
                    return;

                if (check.Status != AsyncOperationStatus.Succeeded)
                    throw check.OperationException ?? new Exception("Could not check the catalog.");

                if (check.Result.Count == 0)
                    return;

                var update = Addressables.UpdateCatalogs(check.Result, false);

                try
                {
                    await update.Task;

                    if (update.Status != AsyncOperationStatus.Succeeded)
                        throw update.OperationException ?? new Exception("Could not update the catalog.");
                }
                finally
                {
                    if (update.IsValid())
                        Addressables.Release(update);
                }
            }
            finally
            {
                if (check.IsValid())
                    Addressables.Release(check);
            }
        }
    }
}

