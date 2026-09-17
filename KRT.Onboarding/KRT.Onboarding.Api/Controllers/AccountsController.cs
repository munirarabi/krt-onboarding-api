using KRT.Onboarding.Api.Models.Requests;
using KRT.Onboarding.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace KRT.Onboarding.Api.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    // POST /api/accounts
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateAccountRequest request,
        CancellationToken cancellationToken)
    {
        var account = await _accountService.CreateAsync(
            request.HolderName,
            request.Cpf,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = account.Id },
            account);
    }

    // GET /api/accounts
    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var accounts = await _accountService.GetAllAsync(
            cancellationToken);

        return Ok(accounts);
    }

    // GET /api/accounts/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var account = await _accountService.GetByIdAsync(
            id,
            cancellationToken);

        if (account is null)
        {
            return NotFound();
        }

        return Ok(account);
    }

    // PUT /api/accounts/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateAccountRequest request,
        CancellationToken cancellationToken)
    {
        var account = await _accountService.UpdateAsync(
            id,
            request.HolderName,
            request.Status,
            cancellationToken);

        if (account is null)
        {
            return NotFound();
        }

        return Ok(account);
    }

    // DELETE /api/accounts/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _accountService.DeleteAsync(
            id,
            cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}