using System.Threading;
using System.Threading.Tasks;
using Contoso.Core;
using Contoso.Core.Domain;
using Contoso.Core.Interfaces.Repositories;
using LanguageExt;
using MediatR;
using static Contoso.Validators;
using static LanguageExt.Prelude;

namespace Contoso.Application.Instructors.Commands
{
    public class UpdateInstructorHandler : IRequestHandler<UpdateInstructor, Either<Error, Task>>
    {
        private readonly IInstructorRepository _instructorRepository;

        public UpdateInstructorHandler(IInstructorRepository instructorRepository) => 
            _instructorRepository = instructorRepository;

        public async Task<Either<Error, Task>> Handle(UpdateInstructor request, CancellationToken cancellationToken) =>
            (await Validate(request))
                .Map(i => ApplyUpdate(i, request))
                .ToEither<Task>();

        private Task ApplyUpdate(Instructor instructor, UpdateInstructor update)
        {                
            instructor.FirstName = update.FirstName;
            instructor.LastName = update.LastName;
            return _instructorRepository.Update(instructor);
        }

        private async Task<Validation<Error, Instructor>> Validate(UpdateInstructor update) => 
            (ValidateFirstName(update), ValidateLastName(update), await InstructorMustExist(update))
                .Apply((first, last, instructor) => instructor);

        private Validation<Error, string> ValidateFirstName(UpdateInstructor update) =>
            NotEmpty(update.FirstName)
                .Bind(NotLongerThan(50));

        private Validation<Error, string> ValidateLastName(UpdateInstructor update) =>
            NotEmpty(update.LastName)
                .Bind(NotLongerThan(50));

        private async Task<Validation<Error, Instructor>> InstructorMustExist(UpdateInstructor update)
        {
            return await (_instructorRepository.Get(update.InstructorId)).Match(
                Some: instructor => Success<Error, Instructor>(instructor),
                None: () => Fail<Error, Instructor>($"Instructor with Id: {update.InstructorId} does not exist."));                 
        }
    }
}
