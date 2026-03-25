using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data_Access_Layer.Configurations
{
    public class MessageConfigurations:IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            // Sender İlişkisi
            builder.HasOne(m => m.Sender)            
                   .WithMany(u => u.SentMessages)    
                   .HasForeignKey(m => m.SenderId)   
                   .OnDelete(DeleteBehavior.Restrict); 

            // Receiver İlişkisi
            builder.HasOne(m => m.Receiver)       
                   .WithMany(u => u.ReceivedMessages)
                   .HasForeignKey(m => m.ReceiverId) 
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
