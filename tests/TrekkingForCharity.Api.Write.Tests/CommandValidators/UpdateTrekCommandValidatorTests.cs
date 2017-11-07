﻿// Copyright 2017 Trekking for Charity
// This file is part of TrekkingForCharity.Api.
// TrekkingForCharity.Api is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// TrekkingForCharity.Api is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with TrekkingForCharity.Api. If not, see http://www.gnu.org/licenses/.

using System.Linq;
using TrekkingForCharity.Api.Write.Commands;
using TrekkingForCharity.Api.Write.CommandValidators;
using Xunit;

namespace TrekkingForCharity.Api.Write.Tests.CommandValidators
{
    public class UpdateTrekCommandValidatorTests
    {
        [Fact]
        public void ShouldNotErrorWhenModelIsValid()
        {
            var validator = new UpdateTrekCommandValidator();
            var command = new UpdateTrekCommand
            {
                Name = "Trek Name",
                Description = "Trek Description"
            };
            var result = validator.Validate(command);
            Assert.True(result.IsValid);
            Assert.False(result.Errors.Any());
        }

        [Fact]
        public void ShouldHaveErrorWhenNameIsNull()
        {
            var validator = new UpdateTrekCommandValidator();
            var command = new UpdateTrekCommand { Description = "Trek Description" };
            var result = validator.Validate(command);
            Assert.False(result.IsValid);
            Assert.True(result.Errors.Any(o => o.PropertyName == "Name"));
        }

        [Fact]
        public void ShouldHaveErrorWhenDescriptionIsNull()
        {
            var validator = new UpdateTrekCommandValidator();
            var command = new UpdateTrekCommand { Name = "Trek Name" };
            var result = validator.Validate(command);
            Assert.False(result.IsValid);
            Assert.True(result.Errors.Any(o => o.PropertyName == "Description"));
        }
    }
}