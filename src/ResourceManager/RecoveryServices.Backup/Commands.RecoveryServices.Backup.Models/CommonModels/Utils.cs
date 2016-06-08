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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Azure.Management.RecoveryServices.Backup.Models;

namespace Microsoft.Azure.Commands.RecoveryServices.Backup.Cmdlets.Models
{
    /// <summary>
    /// Backup policy constants.
    /// </summary>
    public class PolicyConstants
    {
        /// <summary>
        /// Maximum allowed duration length of retention.
        /// </summary>
        public const int MaxAllowedRetentionDurationCount = 9999;

        /// <summary>
        /// Maximum number of days in a month.
        /// </summary>
        public const int MaxAllowedDateInMonth = 28;

        // day constants
        public const int NumOfDaysInWeek = 7;
        public const int NumOfDaysInMonth = 31;
        public const int NumOfDaysInYear = 366;

        // week constants
        public const int NumOfWeeksInMonth = 4;
        public const int NumOfWeeksInYear = 52;

        // month constants
        public const int NumOfMonthsInYear = 12;
    }

    /// <summary>
    /// Trace utilities.
    /// </summary>
    public class TraceUtils
    {
        /// <summary>
        /// Returns a string which contains an enumeration of the given input list.
        /// </summary>
        /// <typeparam name="T">Type of the object in the list</typeparam>
        /// <param name="objList">List of input objects</param>
        /// <returns></returns>
        public static string GetString<T>(IEnumerable<T> objList)
        {
            return (objList == null) ? "null" : "{" + string.Join(", ", objList.Select(e => e.ToString())) + "}";
        }
    }

    /// <summary>
    /// ARM ID utilities.
    /// </summary>
    public class IdUtils
    {
        static readonly string UriFormat = @"/Subscriptions/(?<subscriptionsId>.+)/resourceGroups" +
            @"/(?<resourceGroupName>.+)/providers/(?<providersName>.+)/vaults/(?<BackupVaultName>.+)" +
            "/backupFabrics/(?<BackupFabricName>.+)/protectionContainers/(?<containersName>.+)";
        static readonly Regex ResourceGroupRegex = new Regex(UriFormat, RegexOptions.Compiled);
        const string NameDelimiter = ";";
        const string IdDelimiter = "/";

        public class IdNames
        {
            public const string SubscriptionId = "Subscriptions";
            public const string ResourceGroup = "resourceGroups";
            public const string Provider = "providers";
            public const string Vault = "vaults";
            public const string BackupFabric = "backupFabrics";
            public const string ProtectionContainerName = "protectionContainers";
            public const string ProtectedItemName = "protectedItems";
        }

        /// <summary>
        /// Fetches the resource group name embedded in the ARM ID by parsing.
        /// </summary>
        /// <param name="id">ARM ID to parse</param>
        /// <returns></returns>
        public static string GetResourceGroupName(string id)
        {
            var match = ResourceGroupRegex.Match(id);
            if (match.Success)
            {
                var vmUniqueName = match.Groups["containersName"];
                if (vmUniqueName != null && vmUniqueName.Success)
                {
                    var vmNameInfo = vmUniqueName.Value.Split(NameDelimiter.ToCharArray());
                    if (vmNameInfo.Length == 3)
                    {
                        return vmNameInfo[1];
                    }
                    else if (vmNameInfo.Length == 4)
                    {
                        return vmNameInfo[2];
                    }
                    else
                    {
                        throw new Exception("Container name not in the expected format");
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Fetches the value embedded in the ARM ID, identified the provided input id, in the ARM ID.
        /// </summary>
        /// <param name="id">The ARM input ID</param>
        /// <param name="idName">Name of the value to be returned</param>
        /// <returns></returns>
        public static string GetValueByName(string id, string idName)
        {
            var parts = id.Split(IdDelimiter.ToArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index % 2)
                .ToList();

            var dict = parts[0].ToList().Zip(parts[1].ToList(), (k, v) => new { k, v })
                               .ToDictionary(x => x.k.Value, x => x.v.Value);

            return dict[idName];
        }

        /// <summary>
        /// Parses the name from the fully qualified ARM ID.
        /// URI format: Type;Name
        /// </summary>
        /// <param name="uri">Uri to be parsed</param>
        /// <returns></returns>
        public static string GetNameFromUri(string uri)
        {
            return uri.Substring(uri.IndexOf(NameDelimiter) + 1);
        }

        public static string GetVmNameFromContainerUri(string uri, int partIndex)
        {
            return uri.Split(NameDelimiter.ToCharArray())[4];
        }
    }

    /// <summary>
    /// Enum utilities.
    /// </summary>
    public class EnumUtils
    {
        /// <summary>
        /// Gets the enum of type T given the string equivalent.
        /// </summary>
        /// <typeparam name="T">Type of the enum represented by the string</typeparam>
        /// <param name="enumValue">String to be parsed</param>
        /// <returns></returns>
        public static T GetEnum<T>(string enumValue)
        {
            return (T)Enum.Parse(typeof(T), enumValue);
        }
    }

    /// <summary>
    /// Conversion utilities.
    /// </summary>
    public class ConversionUtils
    {
        /// <summary>
        /// Returns the PS backup management type given the service client defined backup management type.
        /// </summary>
        /// <param name="backupManagementType">Service client backup management type</param>
        /// <returns>PowerShell backup management type</returns>
        public static BackupManagementType GetPsBackupManagementType(string backupManagementType)
        {
            Microsoft.Azure.Management.RecoveryServices.Backup.Models.BackupManagementType providerType
                = EnumUtils.GetEnum<Microsoft.Azure.Management.RecoveryServices.Backup.Models.BackupManagementType>(backupManagementType);

            switch (providerType)
            {
                case Microsoft.Azure.Management.RecoveryServices.Backup.Models.BackupManagementType.AzureIaasVM:
                    return BackupManagementType.AzureVM;
                case Microsoft.Azure.Management.RecoveryServices.Backup.Models.BackupManagementType.MAB:
                    return BackupManagementType.MARS;
                case Microsoft.Azure.Management.RecoveryServices.Backup.Models.BackupManagementType.DPM:
                    return BackupManagementType.SCDPM;
                case Microsoft.Azure.Management.RecoveryServices.Backup.Models.BackupManagementType.AzureBackupServer:
                    return BackupManagementType.AzureBackupServer;
                default:
                    throw new Exception("Unsupported BackupManagmentType: " + backupManagementType);
            }
        }

        /// <summary>
        /// Returns the PS backup management type given the service client defined container type.
        /// </summary>
        /// <param name="containerType">Service client container type</param>
        /// <returns>PowerShell container type</returns>
        public static ContainerType GetPsContainerType(string containerType)
        {
            if (containerType == "Microsoft.ClassicCompute/virtualMachines" ||
                containerType == "Microsoft.Compute/virtualMachines")
            {
                return ContainerType.AzureVM;
            }
            else if (containerType == Microsoft.Azure.Management.RecoveryServices.Backup.Models.ContainerType.Windows.ToString())
            {
                return ContainerType.Windows;
            }
            else
            {
                throw new Exception("Unsupported ContainerType: " + containerType);
            }
        }

        /// <summary>
        /// Returns the PS backup management type given the service client defined workload type.
        /// </summary>
        /// <param name="workloadType">Service client workload type</param>
        /// <returns>PowerShell workload type</returns>
        public static WorkloadType GetPsWorkloadType(string workloadType)
        {
            if (workloadType == Microsoft.Azure.Management.RecoveryServices.Backup.Models.WorkloadType.VM)
            {
                return WorkloadType.AzureVM;
            }
            else
            {
                throw new Exception("Unsupported WorkloadType: " + workloadType);
            }
        }
    }
}
