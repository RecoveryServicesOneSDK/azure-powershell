﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Azure.Management.RecoveryServices.Models;

namespace Microsoft.Azure.Commands.RecoveryServices
{
    /// <summary>
    /// Retrieves Azure Recovery Services Vault.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "AzureRmRecoveryServicesBackupProperties")]
    public class SetAzureRmRecoveryServicesBackupProperties : RecoveryServicesCmdletBase
    {
        #region Parameters
        /// <summary>
        /// Gets or sets vault Object.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public ARSVault Vault { get; set; }
        /// <summary>
        /// Gets or sets BackupStorageRedundancy type.
        /// </summary>
        [Parameter(Mandatory = false)]
        public string BackupStorageRedundancy { get; set; }
        /// <summary>
        /// Gets or sets BackupStorageDeduplication type.
        /// </summary>
        [Parameter(Mandatory = false)]
        public string BackupStorageDeduplication { get; set; }

        #endregion Parameters

        /// <summary>
        /// ProcessRecord of the command.
        /// </summary>
        public override void ExecuteCmdlet()
        {
            try
            {
                // If both attributes are not null
                if (!(string.IsNullOrEmpty(this.BackupStorageRedundancy) && string.IsNullOrEmpty(this.BackupStorageDeduplication)))
                {
                    UpdateVaultStorageTypeRequest vaultStorageRequest = new UpdateVaultStorageTypeRequest();
                    vaultStorageRequest.Properties = new StorageTypeProperties();
                    vaultStorageRequest.Properties.StorageModelType = this.BackupStorageRedundancy;
                    vaultStorageRequest.Properties.DedupState = this.BackupStorageDeduplication;
                    AzureOperationResponse storageResponse = RecoveryServicesClient.UpdateVaultStorageType(this.Vault.ResouceGroupName, this.Vault.Name, vaultStorageRequest);
                }
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }
    }
}
