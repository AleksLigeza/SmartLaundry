using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using SmartLaundry.Data;
using SmartLaundry.Models;
using SmartLaundry.Models.AccountViewModels;
using Xunit;

namespace SmartLaundry.Tests.Integration
{
    public class AccountControllerTests : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _dbContext;
        private readonly TestFixture _fixture;

        public AccountControllerTests(TestFixture fixture)
        {
            _client = fixture.Client;
            _dbContext = fixture.Context;
            _fixture = fixture;
        }

    }
}