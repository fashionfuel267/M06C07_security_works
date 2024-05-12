using M06C07_security.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace M06C07_security.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("res")]
    public class ItemsController : ControllerBase
    {
        private readonly dbHealthContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;
        public ItemsController(dbHealthContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems()
        {
            return await _context.Items.ToListAsync();
        }

        // GET: api/Items/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(long id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        // PUT: api/Items/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(long id, Item item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Items
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public IActionResult PostItem()
        {
            try
            {
                var httpreq = HttpContext.Request.Body;

                var file = Request.Form.Files[0];
                string rootPtah = Path.Combine(this._env.WebRootPath, "ProductImage");

                if (!Directory.Exists(rootPtah))
                {
                    Directory.CreateDirectory(rootPtah);
                }
                if (file.Length > 0)
                {
                    var pid = Request.Form["Pname"].ToString();
                    var price = Request.Form["pr"].ToString();
                    var cid = Request.Form["cid"].ToString();
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string ext = Path.GetExtension(fileName);
                    string filewithoutext = Path.GetFileNameWithoutExtension(fileName);
                    string filepath = Path.Combine(rootPtah, (filewithoutext + "_" + pid + ext));
                    using (var stream = new FileStream(filepath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    string imagePath = "/ProductImage/" + filewithoutext + "_" + pid + ext;
                    var prd = new Item
                    {


                        ImagePath = imagePath,
                        Name = pid,
                        Price = float.Parse(price),
                        CatId = Int32.Parse(cid)
                    };
                    _context.Items.Add(prd);
                    if (_context.SaveChanges() > 0)
                    {
                        return Created("api/items", prd);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            // return Ok(new { resul = "Successfuly created" });
            return BadRequest();
        }

        // DELETE: api/Items/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Item>> DeleteItem(long id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return item;
        }

        private bool ItemExists(long id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }
}
