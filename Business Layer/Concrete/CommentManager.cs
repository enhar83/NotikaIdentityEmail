using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Business_Layer.Abstract;
using Business_Layer.Exceptions;
using Data_Access_Layer.Abstract;
using Entity_Layer.DTOs.CommentDtos;
using Entity_Layer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business_Layer.Concrete
{
    public class CommentManager : ICommentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        public CommentManager(IUnitOfWork uow, IMapper mapper, UserManager<AppUser> userManager)
        {
            _uow = uow;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task ComposeCommentAsync(ComposeCommentDto composeCommentDto)
        {
            var senderId = await _userManager.FindByIdAsync(composeCommentDto.SenderId.ToString());
            if (senderId == null)
                throw new LogicException("SenderId","Gönderen kullanıcı bulunamadı.");

            var comment = _mapper.Map<Comment>(composeCommentDto);
            comment.SenderId = composeCommentDto.SenderId;
            comment.Date = DateTime.Now;

            await TInsertAsync(comment);
        }

        public async Task<List<CommentListDto>> GetCommentListAsync(Guid userId)
        {
            var query = _uow.Comments.GetWhere(c => c.SenderId == userId);

            return await query
                .ProjectTo<CommentListDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(c => c.Date)
                .ToListAsync();
        }

        public async Task<List<CommentListForAdminDto>> GetCommentListForAdmin()
        {
            var query = _uow.Comments.GetWhere();

            return await query 
                .ProjectTo<CommentListForAdminDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(c => c.Date)
                .ToListAsync();
        }

        public async Task<List<CommentListForForumDto>> GetCommentListForForumAsync()
        {
            var query = _uow.Comments.GetWhere(c=>c.CommentStatus == true);

            return await query
                .ProjectTo<CommentListForForumDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(c => c.Date)
                .ToListAsync();
        }

        public void TDelete(Comment entity)
        {
            _uow.Comments.Delete(entity);
            _uow.SaveAsync();
        }

        public async Task<Comment?> TGetByIdAsync(Guid id)
        {
            return await _uow.Comments.GetByIdAsync(id);
        }

        public async Task<List<Comment>> TGetListAsync()
        {
            return await _uow.Comments.GetListAsync();
        }

        public async Task TInsertAsync(Comment entity)
        {
            await _uow.Comments.InsertAsync(entity);
            await _uow.SaveAsync();
        }

        public void TUpdate(Comment entity)
        {
            _uow.Comments.Update(entity);

        }

        public async Task UpdateCommentStatusAsync(UpdateCommentStatusDto updateCommentStatusDto)
        {
            var comment = await TGetByIdAsync(updateCommentStatusDto.Id);
            if (comment == null)
                throw new LogicException("Id", "Yorum bulunamadı.");

            comment.CommentStatus = updateCommentStatusDto.CommentStatus;
            
            TUpdate(comment);
            await _uow.SaveAsync();
        }
    }
}
