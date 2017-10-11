// Copyright 2017 Trekking for Charity
// This file is part of TrekkingForCharity.Api.
// TrekkingForCharity.Api is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// TrekkingForCharity.Api is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with TrekkingForCharity.Api. If not, see http://www.gnu.org/licenses/.

using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using TrekkingForCharity.Api.App.Helpers;
using TrekkingForCharity.Api.Read;

namespace TrekkingForCharity.Api.App.RestfulEndpoints
{
    public static class GetTrekById
    {
        [FunctionName("GetTrekById")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "treks/{trekId}")] HttpRequestMessage req,
            [Table("trek", Connection = "")] IQueryable<Trek> trekTable,
            string trekId,
            TraceWriter log)
        {
            var treks = trekTable.Where(w => w.RowKey == trekId).ToList();

            return treks.Count != 1
                ? req.CreateResponse(HttpStatusCode.NotFound)
                : req.CreateResponseCamelCase(treks.First());
        }
    }
}