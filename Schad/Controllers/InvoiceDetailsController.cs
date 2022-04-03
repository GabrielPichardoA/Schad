using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SchadModels.Models;

namespace Schad.Controllers
{
    public class InvoiceDetailsController : Controller
    {
        private InvoiceEntities db = new InvoiceEntities();

        // GET: InvoiceDetails
        public ActionResult Index()
        {
            var invoiceDetails = db.InvoiceDetails.Include(i => i.Invoice);
            return View(invoiceDetails.ToList());
        }

        // GET: InvoiceDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceDetail invoiceDetail = db.InvoiceDetails.Find(id);
            if (invoiceDetail == null)
            {
                return HttpNotFound();
            }
            return View(invoiceDetail);
        }

        // GET: InvoiceDetails/Create
        public ActionResult Create()
        {
            ViewBag.InvoiceId = new SelectList(db.Invoices, "Id", "Id");
            return View();
        }

        public ActionResult CreateWithId(int? id)
        {
            ViewBag.InvoiceId = id;
            return View();
        }

        delegate Invoice UpdateInvoice(List<InvoiceDetail> invoiceList, Invoice invoice);

        static Invoice SaveInvoice(List<InvoiceDetail> invoiceList, Invoice invoice)
        {
            invoice.SubTotal = 0;
            invoice.TotalItbis = 0;
            invoice.Total = 0;
            invoiceList.ForEach(x =>
            {
                invoice.SubTotal = invoice.SubTotal + x.SubTotal;
                invoice.TotalItbis = invoice.TotalItbis + x.TotalItbis;
                invoice.Total = invoice.Total + x.Total;
            });

            return invoice;
        }

        // POST: InvoiceDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,InvoiceId,Qty,Price,TotalItbis,SubTotal,Total")] InvoiceDetail invoiceDetail)
        {
            if (ModelState.IsValid)
            {
                db.InvoiceDetails.Add(invoiceDetail);
                db.SaveChanges();
            }

            List<InvoiceDetail> invoiceList = db.InvoiceDetails.Where(x => x.InvoiceId == invoiceDetail.InvoiceId).ToList();
            Invoice invoice = db.Invoices.Find(invoiceDetail.InvoiceId);

            UpdateInvoice newInvoice = SaveInvoice;
            newInvoice(invoiceList, invoice);

            db.SaveChanges();

            ViewBag.InvoiceId = new SelectList(db.Invoices, "Id", "Id", invoiceDetail.InvoiceId);
            return RedirectToAction("Index", "Invoices");
        }

        // POST: InvoiceDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWithId([Bind(Include = "Id,InvoiceId,Qty,Price,TotalItbis,SubTotal,Total")] InvoiceDetail invoiceDetail)
        {
            if (ModelState.IsValid)
            {
                db.InvoiceDetails.Add(invoiceDetail);
                db.SaveChanges();
            }

            List<InvoiceDetail> invoiceList = db.InvoiceDetails.Where(x => x.InvoiceId == invoiceDetail.InvoiceId).ToList();
            Invoice invoice = db.Invoices.Find(invoiceDetail.InvoiceId);

            UpdateInvoice newInvoice = SaveInvoice;
            newInvoice(invoiceList, invoice);

            db.SaveChanges();

            ViewBag.InvoiceId = new SelectList(db.Invoices, "Id", "Id", invoiceDetail.InvoiceId);
            return RedirectToAction("Index", "Invoices");
        }

        // GET: InvoiceDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceDetail invoiceDetail = db.InvoiceDetails.Find(id);
            if (invoiceDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.InvoiceId = new SelectList(db.Invoices, "Id", "Id", invoiceDetail.InvoiceId);
            return View(invoiceDetail);
        }

        // POST: InvoiceDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,InvoiceId,Qty,Price,TotalItbis,SubTotal,Total")] InvoiceDetail invoiceDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invoiceDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.InvoiceId = new SelectList(db.Invoices, "Id", "Id", invoiceDetail.InvoiceId);
            return View(invoiceDetail);
        }

        // GET: InvoiceDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceDetail invoiceDetail = db.InvoiceDetails.Find(id);
            if (invoiceDetail == null)
            {
                return HttpNotFound();
            }
            return View(invoiceDetail);
        }

        // POST: InvoiceDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InvoiceDetail invoiceDetail = db.InvoiceDetails.Find(id);
            db.InvoiceDetails.Remove(invoiceDetail);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult GoToInvoices(int id)
        {
            return RedirectToAction("Details/"+id, "Invoices");
        }
    }
}
