using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http;

namespace IN.IP.Common.Func.ConvertEncoding
{
    public static class ConvertEncodingFunction
    {
        [FunctionName("ConvertEncoding")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] Input input,
            ILogger log)
        {
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Encoding inputEncoding = null;


            if (input == null || String.IsNullOrWhiteSpace(input?.Text)|| String.IsNullOrWhiteSpace(input?.EncodingInput)|| String.IsNullOrWhiteSpace(input?.EncodingOutput))
            {
                return new BadRequestObjectResult("Please pass text/encodingOutput properties in the input Json object.");
            }

            try
            {
                string encodingInput = input.EncodingInput;
                inputEncoding = Encoding.GetEncoding(name: encodingInput);
            }
            catch (ArgumentException)
            {
                return new BadRequestObjectResult("Input charset value '" + input.EncodingInput + "' is not supported. Supported value are listed at https://msdn.microsoft.com/en-us/library/system.text.encoding(v=vs.110).aspx.");
            }

            Encoding encodingOutput = null;
            try
            {
                string outputEncoding = input.EncodingOutput;
                encodingOutput = Encoding.GetEncoding(outputEncoding);
            }
            catch (ArgumentException)
            {
                return new BadRequestObjectResult("Output charset value '" + input.EncodingOutput + "' is not supported. Supported value are listed at https://msdn.microsoft.com/en-us/library/system.text.encoding(v=vs.110).aspx.");
            }

            var outputBytes = Encoding.Convert(srcEncoding: inputEncoding, dstEncoding: encodingOutput, bytes: Convert.FromBase64String(input.Text));
            var base64String = Convert.ToBase64String(outputBytes);
            return new FileContentResult(encodingOutput.GetBytes(base64String), "application/octetstream");
//            var outputBytes = Encoding.Convert(srcEncoding: inputEncoding, dstEncoding: encodingOutput, bytes: Convert.FromBase64String(input.Text));
            
  //          log.LogInformation(encodingOutput.GetString(outputBytes));

//            long arrayLength = (long)((4.0d / 3.0d) * outputBytes.Length);

//            if (arrayLength % 4 != 0)
//            {
//                arrayLength += 4 - arrayLength % 4;
//            }

//            char[] base64Array = new char[arrayLength];
//            Convert.ToBase64CharArray(outputBytes, 0, outputBytes.Length, base64Array, 0);
//            return new FileContentResult(base64Array.Select(c => (byte)c).ToArray(), "application/octet-stream");


        }
    }

    
    public class Input
    {
        [Required]
        public string Text { get; set; }
        [Required]
        public string EncodingInput { get; set; }
        [Required]
        public string EncodingOutput { get; set; }
        
    }
}

