using System;
using LibGit2Sharp;

namespace ConvetionalCommitApi
{
    public class Commit
    {
        public String message { get; set; }
        
        public String type { get; set; }

        public Signature commiter{ get; set; }

        public Signature author{ get; set; }
        
    }
}