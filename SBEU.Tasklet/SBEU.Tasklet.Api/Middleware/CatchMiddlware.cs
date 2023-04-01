using AutoMapper;
using SBEU.Exceptions;
using SBEU.Response;
using SBEU.Tasklet.DataLayer.DataBase.Entities;
using SBEU.Tasklet.Models.Responses;

namespace SBEU.Tasklet.Api.Middleware
{
    public class CatchMiddlware
    {
        private readonly IMapper _mapper;

        public CatchMiddlware(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<BaseResponse<Dto>> Try<T, Dto>(Task<T> act, XIdentityUser? user)
        {
            BaseResponse<Dto> xResult;
            try
            {
                if (user == null)
                {
                    return new BaseResponse<Dto>()
                    {
                        Code = 401,
                        Status = false,
                        Error = new BaseError()
                        {
                            Message = "Ошибка авторизации пользователя",
                            Code = "401",
                            Exception = new BaseExceptionError<List<DetailError>>()
                            {
                                Exception = "SBEU.CapiChat.Admin.Service.ControllerExt",
                                Message = "Authorization error",
                                Details = null
                            }
                        }
                    };
                }

                var result = await act;
                var dto = _mapper.Map<Dto>(result);
                xResult = new BaseResponse<Dto>()
                {
                    Result = dto,
                    Code = 200,
                    Status = true
                };
            }
            catch (EntityNotFoundException ex)
            {
                xResult = new BaseResponse<Dto>()
                {
                    Code = 404,
                    Status = false,
                    Error = new BaseError()
                    {
                        Code = "404",
                        Message = "Новость не найдена",
                        Exception = new BaseExceptionError<List<DetailError>>()
                        {
                            Exception = ex.Source,
                            Message = ex.Message
                        }
                    }
                };
            }
            catch (Npgsql.NpgsqlException ex)
            {
                xResult = new BaseResponse<Dto>()
                {
                    Code = 400,
                    Status = false,
                    Error = new BaseError()
                    {
                        Code = "400",
                        Message = "Произошла внутренняя ошибка базы данных при обработке запроса",
                        Exception = new BaseExceptionError<List<DetailError>>()
                        {
                            Exception = ex?.Source ?? "",
                            Message = ex.Message,
                            Details = null
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                xResult = new BaseResponse<Dto>()
                {
                    Code = 400,
                    Status = false,
                    Error = new BaseError()
                    {
                        Code = "400",
                        Message = "Произошла ошибка на стороне сервера",
                        Exception = new BaseExceptionError<List<DetailError>>()
                        {
                            Exception = ex?.Source ?? "",
                            Message = ex.Message,
                            Details = null
                        }
                    }
                };
            }
            return xResult;
        }

        public async Task<BaseResponse<T>> Try<T>(Task act, XIdentityUser? user)
        {
            BaseResponse<T> xResult;
            try
            {
                if (user == null)
                {
                    return new BaseResponse<T>()
                    {
                        Code = 401,
                        Status = false,
                        Error = new BaseError()
                        {
                            Message = "Ошибка авторизации пользователя",
                            Code = "401",
                            Exception = new BaseExceptionError<List<DetailError>>()
                            {
                                Exception = "SBEU.CapiChat.Admin.Service.ControllerExt",
                                Message = "Authorization error",
                                Details = null
                            }
                        }
                    };
                }

                await act;
                xResult = new BaseResponse<T>()
                {
                    Result = default(T),
                    Code = 200,
                    Status = true
                };
            }
            catch (EntityNotFoundException ex)
            {
                xResult = new BaseResponse<T>()
                {
                    Code = 404,
                    Status = false,
                    Error = new BaseError()
                    {
                        Code = "404",
                        Message = "Новость не найдена",
                        Exception = new BaseExceptionError<List<DetailError>>()
                        {
                            Exception = ex.Source,
                            Message = ex.Message
                        }
                    }
                };
            }
            catch (Npgsql.NpgsqlException ex)
            {
                xResult = new BaseResponse<T>()
                {
                    Code = 400,
                    Status = false,
                    Error = new BaseError()
                    {
                        Code = "400",
                        Message = "Произошла внутренняя ошибка базы данных при обработке запроса",
                        Exception = new BaseExceptionError<List<DetailError>>()
                        {
                            Exception = ex?.Source ?? "",
                            Message = ex.Message,
                            Details = null
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                xResult = new BaseResponse<T>()
                {
                    Code = 400,
                    Status = false,
                    Error = new BaseError()
                    {
                        Code = "400",
                        Message = "Произошла ошибка на стороне сервера",
                        Exception = new BaseExceptionError<List<DetailError>>()
                        {
                            Exception = ex?.Source ?? "",
                            Message = ex.Message,
                            Details = null
                        }
                    }
                };
            }
            return xResult;
        }

    }
}
