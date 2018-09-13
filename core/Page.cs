using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace flow.core{
    public class Page{
        private readonly HttpContext _context;
        public Page(HttpContext context){
            _context = context;
        }

        public void Write(){
                var path = _context.Request.Path;
                var query = _context.Request.QueryString;
                _context.Response.WriteAsync($"New URL: {path}{query}");

        }
    }

}