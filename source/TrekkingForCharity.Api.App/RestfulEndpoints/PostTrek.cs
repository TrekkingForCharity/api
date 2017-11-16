﻿// Copyright 2017 Trekking for Charity
// This file is part of TrekkingForCharity.Api.
// TrekkingForCharity.Api is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// TrekkingForCharity.Api is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with TrekkingForCharity.Api. If not, see http://www.gnu.org/licenses/.

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using TrekkingForCharity.Api.App.Helpers;
using TrekkingForCharity.Api.Core.Constants;
using TrekkingForCharity.Api.Write.Commands;
using TrekkingForCharity.Api.Write.CommandValidators;
using TrekkingForCharity.Api.Write.Models;

namespace TrekkingForCharity.Api.App.RestfulEndpoints
{
    public static class PostTrek
    {
        [FunctionName("PostTrek")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "post", Route = "treks")] HttpRequestMessage req,
            [Table("trek")] CloudTable trekTable,
            TraceWriter log)
        {
            try
            {
                var principleMaybe = req.Headers.GetCurrentPrinciple();
                if (principleMaybe.HasNoValue)
                {
                    return req.CreateResponse(HttpStatusCode.Forbidden);
                }

                var principle = principleMaybe.Value;
                var userId = principle.Claims.First(x => x.Type == "sub").Value;

                var jsonContent = await req.Content.ReadAsStringAsync();
                var cmd = JsonConvert.DeserializeObject<CreateTrekCommand>(jsonContent);

                var validator = new CreateTrekCommandValidator();
                var validationResult = await validator.ValidateAsync(cmd);
                if (!validationResult.IsValid)
                {
                    return req.CreateApiErrorResponseFromValidateResults(validationResult);
                }

                trekTable.CreateIfNotExists();

                var trek = new Trek(cmd.Name, cmd.Description, cmd.BannerImage, cmd.WhenToStart, userId);

                var result = await trekTable.CreateEntity(trek);
                if (result.IsFailure)
                {
                    return req.CreateApiErrorResponse(
                        ErrorCodes.Creation, "Something went wrong when trying to create the trek");
                }

                return req.CreateSuccessResponseMessage(trek.ToRead());
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}