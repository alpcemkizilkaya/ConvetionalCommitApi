using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ConvetionalCommitApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BranchController : ControllerBase
    {

        private readonly ILogger<BranchController> _logger;

        public BranchController(ILogger<BranchController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public List<Branch> Get(String path,String username,String password)
        {
            var cloneOptions = new CloneOptions()
            {
                CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                {
                    Username = username,
                    Password = password
                }
            };
            var clone = Repository.Clone(path, "tempRepo/", cloneOptions);
            
        
            var list = new List<Branch>();
            using (var repo = new Repository(@clone))
            {
                // Object lookup
                var obj = repo.Lookup("sha");
                var commit = repo.Lookup<LibGit2Sharp.Commit>("sha");
                var tree = repo.Lookup<Tree>("sha");
                // var tag = repo.Lookup<Tag>("sha");

                
                foreach (var branch in repo.Branches.ToList())
                {

                    var branchObj = new Branch()
                    {
                        name = branch.FriendlyName, isRemote = branch.IsRemote
                    };
                    list.Add(branchObj);
                }
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo("tempRepo/");
                setAttributesNormal(di);
                di.Delete(true);

                return list;




            }
        }
        public static void setAttributesNormal(DirectoryInfo dir) {
            foreach (var subDir in dir.GetDirectories())
                
                setAttributesNormal(subDir);
            foreach (var file in dir.GetFiles())
            {
                Console.WriteLine("file"+file.Name);
                file.Attributes = FileAttributes.Normal;
            }
        }
    }
}