using Microsoft.EntityFrameworkCore;
using AutoGens;
partial class Program
{
    public static void SearchStudentGeneral()
    {
        using (bd_storage db = new())
        {
            WriteLine("Provide the ID of the student you want to search:");
            string Id = ReadNonEmptyLine();

            var studentHistory = db.Students
                .Where(s => s.StudentId == Id)
                .Join(
                    db.Requests,
                    student => student.StudentId,
                    request => request.StudentId,
                    (student, request) => new
                    {
                        student.Name,
                        student.LastNameP,
                        student.LastNameM,
                        student.Group,
                        request.RequestId
                    })
                .Join(
                    db.RequestDetails,
                    studentRequest => studentRequest.RequestId,
                    requestDetail => requestDetail.RequestId,
                    (studentRequest, requestDetail) => new
                    {
                        studentRequest.Name,
                        studentRequest.LastNameP,
                        studentRequest.LastNameM,
                        studentRequest.Group,
                        requestDetail.Quantity,
                        requestDetail.ProfessorNip,
                        requestDetail.DispatchTime,
                        requestDetail.ReturnTime,
                        requestDetail.RequestedDate,
                        requestDetail.CurrentDate,
                        requestDetail.EquipmentId
                    })
                .GroupJoin(
                    db.Equipments,
                    requestDetail => requestDetail.EquipmentId,
                    equipment => equipment.EquipmentId,
                    (requestDetail, equipment) => new
                    {
                        requestDetail.Name,
                        requestDetail.LastNameP,
                        requestDetail.LastNameM,
                        requestDetail.Group,
                        requestDetail.Quantity,
                        requestDetail.ProfessorNip,
                        requestDetail.DispatchTime,
                        requestDetail.ReturnTime,
                        requestDetail.RequestedDate,
                        requestDetail.CurrentDate,
                        requestDetail.EquipmentId,
                        EquipmentName = equipment.FirstOrDefault().Name
                    });

            WriteLine($"ToQueryString: {studentHistory.ToQueryString()}");
            // var query = studentHistory.ToQueryString();
            // WriteLine($"Generated SQL Query: {query}");
            
            if (!studentHistory.Any())
            {
                WriteLine("No student or history found");
                return;
            }

            foreach (var result in studentHistory)
            {
                WriteLine($"Name: {result.Name}, Paternal Last Name: {result.LastNameP}, Maternal Last Name: {result.LastNameM}, Group: {result.Group}");
                WriteLine($"Request Detail: {result.EquipmentId}, Equipment Name: {result.EquipmentName}");
                WriteLine($"Quantity: {result.Quantity}");
                WriteLine($"Professor NIP: {result.ProfessorNip}");
                WriteLine($"Dispatch Time: {result.DispatchTime}");
                WriteLine($"Return Time: {result.ReturnTime}");
                WriteLine($"Requested Date: {result.RequestedDate}");
                WriteLine($"Current Date: {result.CurrentDate}");
            }
            
        }
    }

    public static void SearchStudentUsingEquipment(){

    }
}