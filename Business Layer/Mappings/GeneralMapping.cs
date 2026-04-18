using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Entity_Layer.DTOs.AppRoleDtos;
using Entity_Layer.DTOs.AppUserDtos.ConfirmUserDto;
using Entity_Layer.DTOs.AppUserDtos.ForgotPasswordDtos;
using Entity_Layer.DTOs.AppUserDtos.LoginDtos;
using Entity_Layer.DTOs.AppUserDtos.ProfileDtos;
using Entity_Layer.DTOs.AppUserDtos.RegisterDtos;
using Entity_Layer.DTOs.AppUserDtos.UserListDtos;
using Entity_Layer.DTOs.CategoryDtos;
using Entity_Layer.DTOs.CommentDtos;
using Entity_Layer.DTOs.JwtDtos;
using Entity_Layer.DTOs.MessageDtos;
using Entity_Layer.DTOs.NotificationDtos;
using Entity_Layer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Business_Layer.Mappings
{
    //automapper kütüphanesinin bu sınıfı tanıması için mutlaka Profile sınıfından miras alması gerekir. bu miras sayesinde sınıf içerisinde CreateMap gibi özel yetenekler kullanılabilir. 
    public class GeneralMapping :Profile
    {
        //automapper uygulama ilk çalıştığında bu sınıfın içine girer ve Constructor içindeki kuralları okur. kurallar buraya yazılır çünkü uygulama ayağa kalkarken bu sözlük bir kez belleğe alınmalı ve ihtiyaç anında hemen kullanılmalıdır.
        public GeneralMapping()
        {
            /*  UserRegisterDto nesnesi gelirse onu AppUser nesnesine dönüştürmeyi bil, içindeki isimleri karşılaştır; örneğin ikisinde de Name alanı varsa DTO'dakini al ve Entity'dekinin içine kopyala
                ilk parametre (UserRegisterDto): kaynak (source), yani eldeki ham ver.
                ikinci parametre (AppUser): hedef (destination), yani verinin dönüştürülmesini istediğin nihai nesne.
            */
            
            /*
                ReverseMap: çift yönlü bilet gibidir, kuralın sadece soldan sağa değil, sağdan sola da çalışmasını sağlar.
                normalde UserRegisterDto -> AppUser (kayıt olurken kullanılır)
                ReverseMap ile AppUser -> UserRegisterDto (örneğin kullanıcı bilgilerini düzenleme sayfasına gönderirken, dbdeki veriyi tekrar DTO'ya çevirmek için kullanılır.
                soldaki bilgi taşınan, sağdaki ise görülmek istenen son hali gibi düşünebilir.
             */

            CreateMap<UserRegisterDto, AppUser>().ReverseMap();
            CreateMap<UserLoginDto, AppUser>().ReverseMap();

            Guid currentUserId = Guid.Empty;
            CreateMap<Category, CategorySidebarDto>()
                .ForMember(dest => dest.MessageCount, opt => opt.MapFrom(src =>
                    src.Messages.Count(m => m.ReceiverId == currentUserId)));

            /*
                normalde automapper isimleri aynı olan alanları otomatik eşler, ancak isimler farklıysa formember ile bu işlem tamamlanır.
                dest: hedefteki alandır. o alana gider.
                src: destten alınan alanları buradaki istenen alanın içerisine koyar.
             */
            CreateMap<Message, MessageListInboxDto>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.Name + " " + src.Sender.Surname))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ReverseMap();

            CreateMap<Message, MessageListSendboxDto>()
                .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src => src.Receiver.Name + " " + src.Receiver.Surname))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ReverseMap();

            CreateMap<Message, MessageDetailDto>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.Name + " " + src.Sender.Surname))
                .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src => src.Receiver.Name + " " + src.Receiver.Surname))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ReverseMap();

            /*
                ForSourceMember(): kaynak sınıfıta bulunan ancak hedefte karşılık bulunamayan proplar için kullanılır.

                DoNotValidate(): bu propun hedef sınıfta bir karşılığı yok bunu doğrulama demektir.

                Ignore(): bu alanı şimdilik boş bırak, mapping yaparken buraya dokunma demektir. Message entitysi içerisinde bir SenderId, ReceiverId olmasından dolayı kullanılıyor.
                          eğer Ignore() kullanılmazsa, automapper orada bir veri bulamadığı için hata verebilir. bu kısım Controller içerisinde elle doldurulucak.
                            
                          Ancak eğer Message içerisinde SenderId ve ReceiverId olmasaydı otomatik olarak boş bırakılabilirdi. Zaten eşleşme olmadığından dolayı pas geçecekti.
            */
            CreateMap<ComposeMessageDto, Message>()
                .ForSourceMember(src => src.SenderEmail, opt => opt.DoNotValidate())
                .ForSourceMember(src => src.ReceiverEmail, opt => opt.DoNotValidate())
                .ForMember(dest => dest.SenderId, opt => opt.Ignore())
                .ForMember(dest => dest.ReceiverId, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<EditProfileDto, AppUser>().ReverseMap();

            CreateMap<ConfirmUserDto, AppUser>().ReverseMap();

            /*  PasswordHash ile NewPassword'ü ForMember ile eşlemeye kalkılırsa hata yapılır. 
                    * Dtodaki veri: 123456 (açık metin)
                    * Dbdeki veri: AQAAAAEAACcQAAAAE... (hashlenmiş karmaşık metin)
                
                Bu dönüşümü AutoMapper ile değil, Identity içerisinde bulunan ChangePasswordAsync metodu ile yap. 
            */

            CreateMap<CreateRoleDto, AppRole>()
                .ForMember(dest=>dest.Name, opt=>opt.MapFrom(src=>src.RoleName))
                .ReverseMap();

            CreateMap<RoleListDto, AppRole>()
                .ForMember(dest=>dest.Name, opt=>opt.MapFrom(src=>src.RoleName))
                .ReverseMap();

            CreateMap<UpdateRoleDto, AppRole>()
                .ForMember(dest=>dest.Name, opt=>opt.MapFrom(src=>src.RoleName))
                .ReverseMap();

            CreateMap<AppUser, UserListDto>()
                .ForMember(dest=>dest.FullName, opt=> opt.MapFrom(src=>src.Name +" " + src.Surname))
                .ReverseMap();

            CreateMap<AppRole, AssignRoleDto>()
                .ForMember(dest=>dest.RoleId, opt=> opt.MapFrom(src=>src.Id))
                .ForMember(dest=>dest.RoleName, opt=> opt.MapFrom(src=>src.Name))
                .ReverseMap();

            CreateMap<AppUser, UserRoleAssignDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name + " " + src.Surname))
                .ReverseMap();

            CreateMap<AppUser, SimpleUserDto>()
                .ForMember(dest => dest.Token, opt => opt.Ignore()); //token maplenmesin, elle atanacak. çünkü token dbden gelen bir değer değil, login anında otomatik olarak üretiliyor.

            CreateMap<AppUser, ForgotPasswordDto>()
                .ReverseMap();

            CreateMap<AppUser, ResetPasswordDto>()
                .ReverseMap();

            CreateMap<Message, MessageListInHeaderDto>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.Name + " " + src.Sender.Surname))
                .ForMember(dest => dest.SenderImageUrl, opt => opt.MapFrom(src => src.Sender.ImageUrl))
                .ReverseMap();

            CreateMap<Notification, NotificationListInHeaderDto>()
                .ReverseMap();

            CreateMap<Notification, NotificationListDto>()
                .ReverseMap();

            CreateMap<Notification, NotificationDetailDto>()
                .ForMember(dest=>dest.ReceiverName, opt=>opt.MapFrom(src=>src.AppUser.Name + " " + src.AppUser.Surname))
                .ReverseMap();

            CreateMap<ComposeNotificationDto, Notification>()
                .ReverseMap();

            CreateMap<Message, MessageListByCategoryDto>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.Name + " " + src.Sender.Surname))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ReverseMap();

            CreateMap<Comment, CommentListDto>()
                .ForMember(dest=>dest.SenderName, opt=>opt.MapFrom(src=>src.Sender.Name + " " + src.Sender.Surname))
                .ReverseMap();

            CreateMap<ComposeCommentDto, Comment>()
                .ReverseMap();

            CreateMap<Comment, CommentListForAdminDto>()
                .ForMember(dest=>dest.SenderName, opt=>opt.MapFrom(src=>src.Sender.Name + " " + src.Sender.Surname))
                .ReverseMap();

            CreateMap<Comment, CommentListForForumDto>()
                .ForMember(dest=>dest.SenderName, opt=>opt.MapFrom(src=>src.Sender.Name + " " + src.Sender.Surname))
                .ReverseMap();

            CreateMap<SetPasswordDto, AppUser>()
                .ReverseMap();

            CreateMap<Category,CategoryListDto>()
                .ReverseMap();

            CreateMap<ComposeCategoryDto, Category>()
                .ReverseMap();
        }
    }
}

/*
    Mapping Nedir?
        - bir nesnenin içindeki verileri başka bir nesnenin içine kopyalama işlemidir.
        - günlük hayat örneği: elinde bir nüfus cüzdanı (entity) var, üzerinde tc no, ad, soyad vs gibi 20 adet bilgi yazıyor. bir kargo göndermek istiyosun ve kargo formu (DTO) senden sadece ad soyad ve telefon istiyor.
            * nüfus cüzdanındaki ad kısmını bakıp kargo formundaki ad kısmına yazma eylemi Mapping olarak adlandırılır.

    Neden AutoMapper Kullanılır?
        - eğer automapper kullanılmazsa her kayıt işleminde şöyle kodlar yazılmak zorunda olur;
            var user = new AppUser();
            user.Name = registerDto.Name;
            user.Surname = registerDto.Surname;
            user.Email = registerDto.Email;
            user.UserName = registerDto.UserName;
            user.City = registerDto.City;

        - bu yöntemle 50 tane tablo olduğunu düşün, binlerce satır gereksiz atama kodu yazılır. AutoMapper ise der ki sen bana şablonu bir kere ver ben isimleri aynı olanları senin yerine otomatik kopyalarım.

    AutoMapper Nasıl Çalışır?
        - 1. Profile: hangi sınıfın hangi sınıfa dönüşeceği yazılır (kullanım kılavuzu)
        - 2. CreateMap: A sınıfını B sınıfına dönüştürebilirsin komutudur.
        - 3. Map: Gerçekten dönüştürme işlemini başlatan tetikleyicidir.

    Kritik Bilgi
        - eğer isimler birebir aynıysa (Name=Name), automapper hiçbir ek kod yazmaya gerek kalmadan eşleştirmeyi yapar. eğer isimler farklı olsaydı buraya ekstra bir ForMember kuralı eklemek gerekecekti.
 */