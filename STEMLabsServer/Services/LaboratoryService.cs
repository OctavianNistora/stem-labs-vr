using Microsoft.EntityFrameworkCore;
using STEMLabsServer.Data;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Models.Entities;
using STEMLabsServer.Services.Interfaces;

namespace STEMLabsServer.Services;

public class LaboratoryService(MainDbContext context) : ILaboratoryService
{
    public async Task CreateLaboratory(LaboratoryDto laboratoryDto, CancellationToken cancellationToken)
    {
        var newLaboratory = new Laboratory
        {
            Name = laboratoryDto.Name,
            SceneId = laboratoryDto.SceneId,
            CheckListStepCount = laboratoryDto.Steps.Count
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
        var laboratory = await context.Laboratories.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        if (laboratory == null)
        {
            return false;
        }

        laboratory.Name = laboratoryDto.Name;
        laboratory.SceneId = laboratoryDto.SceneId;
        laboratory.CheckListStepCount = laboratoryDto.Steps.Count;

        context.Laboratories.Update(laboratory);

        var steps = await context.LaboratoryChecklistSteps.Where(s => s.LaboratoryId == id)
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
        
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IEnumerable<LaboratoryFullListItemDto>> GetLaboratories(CancellationToken cancellationToken)
    {
        return await context.Laboratories.AsNoTracking()
            .Select(laboratory => new LaboratoryFullListItemDto
            {
                Id = laboratory.Id,
                Name = laboratory.Name,
                SceneId = laboratory.SceneId,
                CheckListStepCount = laboratory.CheckListStepCount
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<IdNameDto>> GetLaboratoriesSimplified(
        CancellationToken cancellationToken)
    {
        return await context.Laboratories.AsNoTracking()
            .Select(laboratory => new IdNameDto
            {
                Id = laboratory.Id,
                Name = laboratory.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<LaboratoryDto?> GetLaboratory(int id, CancellationToken cancellationToken)
    {
        var laboratory = await context.Laboratories.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        if (laboratory == null)
        {
            return null;
        }

        var existingSteps = context.LaboratoryChecklistSteps
            .Where(listStep => listStep.LaboratoryId == id)
            .GroupBy(listStep => listStep.StepNumber,
            (_, group) => group.OrderByDescending(element => element.Version).First())
            .Take(laboratory.CheckListStepCount)
            .ToList()
            .Select(listStep => listStep.Statement)
            .ToList();

        return new LaboratoryDto
        {
            Name = laboratory.Name,
            SceneId = laboratory.SceneId,
            Steps = existingSteps
        };
    }

    public async Task<IEnumerable<string>> GetLaboratorySteps(int sceneId, CancellationToken cancellationToken)
    {
        var laboratory = await context.Laboratories.FirstOrDefaultAsync(l => l.SceneId == sceneId, cancellationToken);
        if (laboratory == null)
        {
            return [];
        }
        
        var checklistSteps = await context.LaboratoryChecklistSteps
            .Where(step => step.LaboratoryId == laboratory.Id)
            .GroupBy(step => step.StepNumber,
                (stepNumber, group) => group.OrderByDescending(step => step.Version).First())
            .ToListAsync(cancellationToken);

        return checklistSteps
            .OrderBy(step => step.StepNumber)
            .Select(step => step.Statement)
            .Take(laboratory.CheckListStepCount);
    }

    public async Task<IEnumerable<IdDateDto>> GetLaboratorySessions(int id, CancellationToken cancellationToken)
    {
        return await context.LaboratorySessions.AsNoTracking()
            .Where(session => session.LaboratoryId == id)
            .Select(session => new IdDateDto
            {
                Id = session.Id,
                Date = session.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}