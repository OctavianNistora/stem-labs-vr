using Microsoft.EntityFrameworkCore;
using STEMLabsServer.Data;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Models.Entities;

namespace STEMLabsServer.Services;

public class LabService(MainDbContext context) : ILabService
{
    public async Task CreateLaboratory(LaboratoryDto laboratoryDto, CancellationToken cancellationToken)
    {
        var newLaboratory = new Laboratory
        {
            Name = laboratoryDto.Name,
            SceneId = laboratoryDto.SceneId,
        };
        
        await context.Laboratories.AddAsync(newLaboratory, cancellationToken);
        
        List<LaboratoryChecklistStep> steps = [];
        for (int i = 0; i < laboratoryDto.Steps.Count; i++)
        {
            var newStep = new LaboratoryChecklistStep
            {
                Laboratory = newLaboratory,
                Version = 1,
                StepNumber = i + 1,
                Statement = laboratoryDto.Steps[i]
            };
            steps.Add(newStep);
        }
        await context.LaboratoryChecklistSteps.AddRangeAsync(steps, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateLaboratory(int id, LaboratoryDto laboratoryDto, CancellationToken cancellationToken)
    {
        var laboratory =
            await context.Laboratories.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        
        if (laboratory == null)
        {
            return false;
        }

        laboratory.Name = laboratoryDto.Name;
        laboratory.SceneId = laboratoryDto.SceneId;
        
        context.Laboratories.Update(laboratory);
        
        var steps = await context.LaboratoryChecklistSteps
            .Where(s => s.LaboratoryId == id)
            .ToListAsync(cancellationToken);
        var currentSteps = steps
            .GroupBy(s => s.StepNumber,
                (_, group) => group.OrderByDescending(groupElement => groupElement.Version).First())
            .OrderBy(step => step.StepNumber).ToList();

        List<LaboratoryChecklistStep> newSteps = new List<LaboratoryChecklistStep>();
        for (int i = 0; i < laboratoryDto.Steps.Count; i++)
        {
            if (i < currentSteps.Count && laboratoryDto.Steps[i] == currentSteps[i].Statement)
            {
                continue;
            }
            
            var newStep = new LaboratoryChecklistStep
            {
                Laboratory = laboratory,
                Version = i < currentSteps.Count ? currentSteps[i].Version + 1 : 1,
                StepNumber = i + 1,
                Statement = laboratoryDto.Steps[i]
            };
            newSteps.Add(newStep);
        }
        
        if (newSteps.Count > 0)
        {
            await context.LaboratoryChecklistSteps.AddRangeAsync(newSteps, cancellationToken);
        }
        
        return true;
    }
}