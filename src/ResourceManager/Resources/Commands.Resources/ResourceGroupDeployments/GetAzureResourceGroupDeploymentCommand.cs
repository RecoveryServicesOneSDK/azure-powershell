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

using Microsoft.Azure.Commands.ResourceManager.Cmdlets.Components;
using Microsoft.Azure.Commands.Resources.Models;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.Resources
{
    /// <summary>
    /// Filters resource group deployments.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureRmResourceGroupDeployment", DefaultParameterSetName = GetAzureResourceGroupDeploymentCommand.DeploymentNameParameterSet), OutputType(typeof(List<PSResourceGroupDeployment>))]
    public class GetAzureResourceGroupDeploymentCommand : ResourcesBaseCmdlet
    {
        /// <summary>
        /// The deployment Id parameter set.
        /// </summary>
        internal const string DeploymentIdParameterSet = "The deployment Id parameter set.";

        /// <summary>
        /// The deployment name parameter set.
        /// </summary>
        internal const string DeploymentNameParameterSet = "The deployment name parameter set.";

        [Parameter(Position = 0, ParameterSetName = GetAzureResourceGroupDeploymentCommand.DeploymentNameParameterSet, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the resource group.")]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Alias("DeploymentName")]
        [Parameter(Position = 1, ParameterSetName = GetAzureResourceGroupDeploymentCommand.DeploymentNameParameterSet, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the resource group deployment.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Alias("DeploymentId", "ResourceId")]
        [Parameter(ParameterSetName = GetAzureResourceGroupDeploymentCommand.DeploymentIdParameterSet, Mandatory = true, HelpMessage = "The fully qualified resource Id of the deployment. example: /subscriptions/{subId}/resourceGroups/{rgName}/providers/Microsoft.Resources/deployments/{deploymentName}")]
        [ValidateNotNullOrEmpty]
        public string Id { get; set; }

        public override void ExecuteCmdlet()
        {
            WriteWarning("The output object type of this cmdlet will be modified in a future release.");
            FilterResourceGroupDeploymentOptions options = new FilterResourceGroupDeploymentOptions()
            {
                ResourceGroupName = ResourceGroupName ?? ResourceIdUtility.GetResourceGroupName(Id),
                DeploymentName = Name ?? (string.IsNullOrEmpty(Id) ? null : ResourceIdUtility.GetResourceName(Id))
            };

            WriteObject(ResourcesClient.FilterResourceGroupDeployments(options), true);
        }
    }
}