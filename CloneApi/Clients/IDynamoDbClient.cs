

using CloneApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloneApi.Clients

{
    public interface IDynamoDbClient
    {
        public Task<UserRequest> GetData(string Id);
        public Task<string> GetLastId();
        public Task<string> GetLastUserMode(string userId);


        public Task<bool> PostDate(UserRequest userRequest);


        public Task<List<UserRequest>> GetAll();



    }
}
