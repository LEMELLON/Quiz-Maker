using AutoMapper;
using QuizMakerEngine.Data.Entities;
using QuizMakerEngine.Dtos;

namespace QuizMakerEngine.Mapping;

public class QuizProfile : Profile
{
    public QuizProfile()
    {
        // Entity -> DTO
        CreateMap<Choice, ChoiceDto>();
        CreateMap<Question, QuestionDto>();
        CreateMap<Quiz, QuizSummaryDto>()
            .ForMember(d => d.QuestionCount, o => o.MapFrom(s => s.Questions.Count));
        CreateMap<Quiz, QuizDetailDto>().IncludeBase<Quiz, QuizSummaryDto>();

        // DTO -> Entity
        CreateMap<CreateChoiceDto, Choice>();
        CreateMap<CreateQuestionDto, Question>();
        CreateMap<CreateQuizDto, Quiz>();
    }
}
