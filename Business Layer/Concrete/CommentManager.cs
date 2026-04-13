using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Business_Layer.Abstract;
using Data_Access_Layer.Abstract;
using Entity_Layer.DTOs.CommentDtos;
using Entity_Layer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business_Layer.Concrete
{
    public class CommentManager : ICommentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public CommentManager(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<List<CommentListDto>> GetCommentListAsync(Guid userId)
        {
            var query = _uow.Comments.GetWhere(c => c.SenderId == userId);

            return await query
                .ProjectTo<CommentListDto>(_mapper.ConfigurationProvider)
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
            _uow.SaveAsync();
        }
    }
}
