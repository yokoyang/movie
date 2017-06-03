using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel.GenericRepository;

namespace DataModel.UnitOfWork
{
    /// <summary>
    /// Unit of Work class responsible for DB transactions
    /// </summary>
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        #region Private member variables...

        private WebApiDbEntities _context = null;
        private GenericRepository<concession> _concessionRepository;
        private GenericRepository<concession_record> _concessionRecordRepository;
        private GenericRepository<genre> _genreRepository;
        private GenericRepository<movie> _movieRepository;
        private GenericRepository<movie_admin> _movieAdminRepository;
        private GenericRepository<net_user> _netUserRepository;
        private GenericRepository<picture> _pictureRepository;
        private GenericRepository<shop_cart_movie> _shopCartMovieRepository;

        #endregion

        public UnitOfWork()
        {
            _context = new WebApiDbEntities();
        }

        #region Public Repository Creation properties...

        public GenericRepository<concession> ConcessionRepository
        {
            get
            {
                if (this._concessionRepository == null)
                    this._concessionRepository = new GenericRepository<concession>(_context);
                return _concessionRepository;
            }
        }

        public GenericRepository<concession_record> ConcessionRecordRepository
        {
            get
            {
                if (this._concessionRecordRepository == null)
                    this._concessionRecordRepository = new GenericRepository<concession_record>(_context);
                return _concessionRecordRepository;
            }
        }


        public GenericRepository<genre> GenreRepository
        {
            get
            {
                if (this._genreRepository == null)
                    this._genreRepository = new GenericRepository<genre>(_context);
                return _genreRepository;
            }
        }

        public GenericRepository<movie> MovieRepository
        {
            get
            {
                if (this._movieRepository == null)
                    this._movieRepository = new GenericRepository<movie>(_context);
                return _movieRepository;
            }
        }


        public GenericRepository<movie_admin> MovieAdminRepository
        {
            get
            {
                if (this._movieAdminRepository == null)
                    this._movieAdminRepository = new GenericRepository<movie_admin>(_context);
                return _movieAdminRepository;
            }
        }

        public GenericRepository<picture> PictureRepository
        {
            get
            {
                if (this._pictureRepository == null)
                    this._pictureRepository = new GenericRepository<picture>(_context);
                return _pictureRepository;
            }
        }

        public GenericRepository<net_user> NetUserRepository
        {
            get
            {
                if (this._netUserRepository == null)
                    this._netUserRepository = new GenericRepository<net_user>(_context);
                return _netUserRepository;
            }
        }

        public GenericRepository<shop_cart_movie> ShopCartMovieRepository
        {
            get
            {
                if (this._shopCartMovieRepository == null)
                    this._shopCartMovieRepository = new GenericRepository<shop_cart_movie>(_context);
                return _shopCartMovieRepository;
            }
        }

        #endregion

        #region Public member methods...

        /// <summary>
        /// Save method.
        /// </summary>
        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                var outputLines = new List<string>();
                foreach (var eve in e.EntityValidationErrors)
                {
                    outputLines.Add(string.Format(
                        "{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:",
                        DateTime.Now,
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        outputLines.Add(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName,
                            ve.ErrorMessage));
                    }
                }
                System.IO.File.AppendAllLines(@"C:\errors.txt", outputLines);

                throw e;
            }
        }

        #endregion

        #region Implementing IDiosposable...

        #region private dispose variable declaration...

        private bool disposed = false;

        #endregion

        /// <summary>
        /// Protected Virtual Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Debug.WriteLine("UnitOfWork is being disposed");
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}