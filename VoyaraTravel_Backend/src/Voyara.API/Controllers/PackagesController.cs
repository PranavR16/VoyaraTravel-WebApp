using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Core;
using Voyara.Core.DTOs.Packages;

namespace Voyara.API.Controllers
{
    [ApiController]
    [Route("api/packages")]
    public class PackagesController(IPackageService packageService) : ControllerBase
    {
        // GET api/packages?category=beach&sortBy=low&page=1&pageSize=12
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PackageFilterDto filter)
        {
            var result = await packageService.GetAllAsync(filter);
            return Ok(result);
        }

        // GET api/packages/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await packageService.GetByIdAsync(id);
            return Ok(result);
        }

        // GET api/packages/category/{category}
        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetByCategory(string category)
        {
            var result = await packageService.GetByCategoryAsync(category);
            return Ok(result);
        }

        // POST api/packages — Admin only
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreatePackageDto dto)
        {
            var result = await packageService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PATCH api/packages/{id} — Admin only
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id,
            [FromBody] UpdatePackageDto dto)
        {
            var result = await packageService.UpdateAsync(id, dto);
            return Ok(result);
        }

        // DELETE api/packages/{id} — Admin only
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await packageService.DeleteAsync(id);
            return NoContent();
        }
    }

    //[ApiController]
    //[Route("api/packages")]
    //public class PackagesController(IPackageService packageService) : ControllerBase
    //{
    //    [HttpGet]
    //    public async Task<IActionResult> GetAll([FromQuery] PackageFilterDto filter)
    //        => Ok(await packageService.GetAllAsync(filter));

    //    [HttpGet("{id}")]
    //    public async Task<IActionResult> GetById(Guid id)
    //        => Ok(await packageService.GetByIdAsync(id));

    //    [HttpGet("category/{category}")]
    //    public async Task<IActionResult> GetByCategory(string category)
    //        => Ok(await packageService.GetByCategoryAsync(category));

    //    [HttpPost]
    //    [Authorize(Roles = "Admin")]
    //    public async Task<IActionResult> Create(CreatePackageDto dto)
    //        => CreatedAtAction(nameof(GetById),
    //            new { id = (await packageService.CreateAsync(dto)).Id },
    //            await packageService.CreateAsync(dto));

    //    [HttpPatch("{id}")]
    //    [Authorize(Roles = "Admin")]
    //    public async Task<IActionResult> Update(Guid id, UpdatePackageDto dto)
    //        => Ok(await packageService.UpdateAsync(id, dto));

    //    [HttpDelete("{id}")]
    //    [Authorize(Roles = "Admin")]
    //    public async Task<IActionResult> Delete(Guid id)
    //    {
    //        await packageService.DeleteAsync(id);
    //        return NoContent();
    //    }
    //}

}
