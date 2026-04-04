using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Exceptions
{
    public class LogicException : Exception
    {
        public string PropertyName { get; } //hangi propta hata oluştuğunu  belirtmek için.

        //costructor içerisinde hata fırlatılırken hem prop adını hem de mesajı alır.
        //base(message) ile mesajı standart exception sınıfı içerisine gönderir.
        public LogicException(string propertyName, string message) : base(message)
        {
            PropertyName = propertyName;
        }
    }
}

/*
    standart hata mesajlarından farkı, hatanın neden kaynaklandığının yanı sıra hangi prop içerisinde oluştuğunu da rapor eder. 
    
    normal bir exception kullanıldığında Controller tarafında sadece bir metin alınır ancak kullanıcıya hata gösterilirken hatanın nerede olduğunu propertyname ile gösterebilirsin.

    bu sınıf projedeki her yerde kullanılabilir. MessageManager, CategoryManager vs. istenilen her sınıf içerisinde hatalar özelleştirilebilir. 
 */
