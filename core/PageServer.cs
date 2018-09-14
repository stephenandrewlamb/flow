using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using CommonMark;
using CommonMark.Syntax;


namespace flow.core{
    public interface IPageServer{
        string Get();
        void SetContext(HttpContext context);
    }
    public class PageServer: IPageServer{
        private HttpContext _context;
        public PageServer(){
        }

        public void SetContext(HttpContext context){
            _context = context;
        }

        public Task Write(){
            var path = _context.Request.Path;
            var query = _context.Request.QueryString;
            var test = @"
            **this is our wiki page**

            test 2
            ======

            ==test 3==
            ";

            var commonMarkSettings = CommonMarkSettings.Default.Clone();
                commonMarkSettings.OutputDelegate = (doc, output, settings) =>
                    new CustomHtmlFormatter(output, settings).WriteDocument(doc);            
            var result = CommonMark.CommonMarkConverter.Convert(test, commonMarkSettings);
            
            //var pipeline = new Markdig.MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            //var result = Markdig.Markdown.ToHtml(test, pipeline);

            //Console.Write(result);
            return _context.Response.WriteAsync($"<html><body><h1>test page</h1>{result} New URL: {path}{query}</body></html>");

        }

        public string Get(){
            var path = _context.Request.Path;
            var query = _context.Request.QueryString;
            var test = @"
** this is our wiki page **

test 2
======

== test 3 ==
            ";

            var commonMarkSettings = CommonMarkSettings.Default.Clone();
                commonMarkSettings.OutputDelegate = (doc, output, settings) =>
                    new CustomHtmlFormatter(output, settings).WriteDocument(doc);            
            var result = CommonMark.CommonMarkConverter.Convert(test, commonMarkSettings);
            
            return $"<html><body><h1>test page</h1>{result} New URL: {path}{query}</body></html>";
        }
    }

    public class CustomHtmlFormatter : CommonMark.Formatters.HtmlFormatter
        {
            public CustomHtmlFormatter(System.IO.TextWriter target, CommonMarkSettings settings)
                : base(target, settings)
            {
            }

            protected override void WriteBlock(Block block, bool isOpening, bool isClosing, out bool ignoreChildNodes)
            {
                if ((block.Tag == BlockTag.FencedCode || block.Tag == BlockTag.IndentedCode) 
                        && !this.RenderPlainTextInlines.Peek())
                {
                    ignoreChildNodes = false;
                    if (isOpening)
                    {
                        this.Write("<?prettify?>");
                    }
                }
                base.WriteBlock(block, isOpening, isClosing, out ignoreChildNodes);
            }
        }

}