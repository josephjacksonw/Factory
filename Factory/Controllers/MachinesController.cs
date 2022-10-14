using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Factory.Models;
using System.Collections.Generic;
using System.Linq;

namespace Factory.Controllers
{
  public class MachinesController : Controller
  {
    private readonly FactoryContext _db;

    public MachinesController(FactoryContext db)
    {
      _db = db;
    }

    public ActionResult Index()
    {
      return View(_db.Machines.ToList().OrderBy(model => model.DueDate).ToList());
    }

    public ActionResult Create()
    {
      ViewBag.EngineerId = new SelectList(_db.Engineers, "EngineerId", "Name");
      return View();
    }

    [HttpPost]
    public ActionResult Create(Machine machine, int EngineerId)
    {
        _db.Machines.Add(machine);
        _db.SaveChanges();
        if (EngineerId != 0)
        {
            _db.EngineerMachine.Add(new EngineerMachine() { EngineerId = EngineerId, MachineId = machine.MachineId });
            _db.SaveChanges();
        }
        return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      var thisMachine = _db.Machines
        .Include(machine => machine.JoinEntities)
        .ThenInclude(join => join.Engineer)
        .FirstOrDefault(machine => machine.MachineId == id);
      return View(thisMachine);
    }

    public ActionResult Edit(int id)
    {
        var thisMachine = _db.Machines.FirstOrDefault(machine => machine.MachineId == id);
        ViewBag.EngineerId = new SelectList(_db.Engineers, "EngineerId", "Name");
        return View(thisMachine);
    }

    [HttpPost]
    public ActionResult Edit(Machine machine/*, int EngineerId*/)
    {
        // if (EngineerId == 0)//this fix (previously !=), prevents it from making multiple copies of machines but we can no longer change the engineer of the machine
        // {
        //   _db.EngineerMachine.Add(new EngineerMachine() { EngineerId = EngineerId, MachineId = machine.MachineId });
        // }
        // 
        /*
        if (EngineerId != 0)
        {
          _db.EngineerMachine.Add(new EngineerMachine() { EngineerId = EngineerId, MachineId = machine.MachineId });
        } */ // this is the previous version that had the make multiple copies bug
        _db.Entry(machine).State = EntityState.Modified;
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    public ActionResult AddEngineer(int id)
    {
        var thisMachine = _db.Machines.FirstOrDefault(machine => machine.MachineId == id);
        ViewBag.EngineerId = new SelectList(_db.Engineers, "EngineerId", "Name");
        return View(thisMachine);
    }

    [HttpPost]
    public ActionResult AddEngineer(Machine machine, int EngineerId)
    {
        foreach(EngineerMachine entry in _db.EngineerMachine)
        {
          if(machine.MachineId == entry.MachineId && EngineerId == entry.EngineerId)
          {
            return RedirectToAction("Index");
          }
        }
        if (EngineerId != 0) 
        {
          _db.EngineerMachine.Add(new EngineerMachine() { EngineerId = EngineerId, MachineId = machine.MachineId });
          _db.SaveChanges();
        }
        return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
        var thisMachine = _db.Machines.FirstOrDefault(machine => machine.MachineId == id);
        return View(thisMachine);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
        var thisMachine = _db.Machines.FirstOrDefault(machine => machine.MachineId == id);
        _db.Machines.Remove(thisMachine);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteEngineer(int joinId)
    {
        var joinEntry = _db.EngineerMachine.FirstOrDefault(entry => entry.EngineerMachineId == joinId);
        _db.EngineerMachine.Remove(joinEntry);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
  }
}