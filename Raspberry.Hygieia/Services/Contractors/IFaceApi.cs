
namespace Raspberry.Hygieia.Services.Contractors
{
    using Raspberry.Hygieia.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    interface IFaceApi
    {
        FaceResponse DetectFace(string imagePath);
    }
}
