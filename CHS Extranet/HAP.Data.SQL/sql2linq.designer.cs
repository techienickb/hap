﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HAP.Data.SQL
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="hap")]
	public partial class sql2linqDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertTrackerEvent(TrackerEvent instance);
    partial void UpdateTrackerEvent(TrackerEvent instance);
    partial void DeleteTrackerEvent(TrackerEvent instance);
    partial void InsertWebTrackerEvent(WebTrackerEvent instance);
    partial void UpdateWebTrackerEvent(WebTrackerEvent instance);
    partial void DeleteWebTrackerEvent(WebTrackerEvent instance);
    partial void InsertTicket(Ticket instance);
    partial void UpdateTicket(Ticket instance);
    partial void DeleteTicket(Ticket instance);
    partial void InsertNote(Note instance);
    partial void UpdateNote(Note instance);
    partial void DeleteNote(Note instance);
    partial void InsertNoteFile(NoteFile instance);
    partial void UpdateNoteFile(NoteFile instance);
    partial void DeleteNoteFile(NoteFile instance);
    #endregion
		
		public sql2linqDataContext() : 
				base("Data Source=cri-svr-002.crick.internal;Initial Catalog=hap;User ID=hap;Password=h" +
						"ap", mappingSource)
		{
			OnCreated();
		}
		
		public sql2linqDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public sql2linqDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public sql2linqDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public sql2linqDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<TrackerEvent> TrackerEvents
		{
			get
			{
				return this.GetTable<TrackerEvent>();
			}
		}
		
		public System.Data.Linq.Table<WebTrackerEvent> WebTrackerEvents
		{
			get
			{
				return this.GetTable<WebTrackerEvent>();
			}
		}
		
		public System.Data.Linq.Table<Ticket> Tickets
		{
			get
			{
				return this.GetTable<Ticket>();
			}
		}
		
		public System.Data.Linq.Table<Note> Notes
		{
			get
			{
				return this.GetTable<Note>();
			}
		}
		
		public System.Data.Linq.Table<NoteFile> NoteFiles
		{
			get
			{
				return this.GetTable<NoteFile>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.TrackerEvents")]
	public partial class TrackerEvent : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _id;
		
		private System.DateTime _LogonDateTime;
		
		private string _Username;
		
		private string _ComputerName;
		
		private System.Nullable<System.DateTime> _LogoffDateTime;
		
		private string _domainname;
		
		private string _ip;
		
		private string _logonserver;
		
		private string _os;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidChanging(int value);
    partial void OnidChanged();
    partial void OnLogonDateTimeChanging(System.DateTime value);
    partial void OnLogonDateTimeChanged();
    partial void OnUsernameChanging(string value);
    partial void OnUsernameChanged();
    partial void OnComputerNameChanging(string value);
    partial void OnComputerNameChanged();
    partial void OnLogoffDateTimeChanging(System.Nullable<System.DateTime> value);
    partial void OnLogoffDateTimeChanged();
    partial void OndomainnameChanging(string value);
    partial void OndomainnameChanged();
    partial void OnipChanging(string value);
    partial void OnipChanged();
    partial void OnlogonserverChanging(string value);
    partial void OnlogonserverChanged();
    partial void OnosChanging(string value);
    partial void OnosChanged();
    #endregion
		
		public TrackerEvent()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int id
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((this._id != value))
				{
					this.OnidChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("id");
					this.OnidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LogonDateTime", DbType="DateTime NOT NULL")]
		public System.DateTime LogonDateTime
		{
			get
			{
				return this._LogonDateTime;
			}
			set
			{
				if ((this._LogonDateTime != value))
				{
					this.OnLogonDateTimeChanging(value);
					this.SendPropertyChanging();
					this._LogonDateTime = value;
					this.SendPropertyChanged("LogonDateTime");
					this.OnLogonDateTimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Username", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Username
		{
			get
			{
				return this._Username;
			}
			set
			{
				if ((this._Username != value))
				{
					this.OnUsernameChanging(value);
					this.SendPropertyChanging();
					this._Username = value;
					this.SendPropertyChanged("Username");
					this.OnUsernameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ComputerName", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string ComputerName
		{
			get
			{
				return this._ComputerName;
			}
			set
			{
				if ((this._ComputerName != value))
				{
					this.OnComputerNameChanging(value);
					this.SendPropertyChanging();
					this._ComputerName = value;
					this.SendPropertyChanged("ComputerName");
					this.OnComputerNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LogoffDateTime", DbType="DateTime")]
		public System.Nullable<System.DateTime> LogoffDateTime
		{
			get
			{
				return this._LogoffDateTime;
			}
			set
			{
				if ((this._LogoffDateTime != value))
				{
					this.OnLogoffDateTimeChanging(value);
					this.SendPropertyChanging();
					this._LogoffDateTime = value;
					this.SendPropertyChanged("LogoffDateTime");
					this.OnLogoffDateTimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_domainname", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string domainname
		{
			get
			{
				return this._domainname;
			}
			set
			{
				if ((this._domainname != value))
				{
					this.OndomainnameChanging(value);
					this.SendPropertyChanging();
					this._domainname = value;
					this.SendPropertyChanged("domainname");
					this.OndomainnameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ip", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string ip
		{
			get
			{
				return this._ip;
			}
			set
			{
				if ((this._ip != value))
				{
					this.OnipChanging(value);
					this.SendPropertyChanging();
					this._ip = value;
					this.SendPropertyChanged("ip");
					this.OnipChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_logonserver", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string logonserver
		{
			get
			{
				return this._logonserver;
			}
			set
			{
				if ((this._logonserver != value))
				{
					this.OnlogonserverChanging(value);
					this.SendPropertyChanging();
					this._logonserver = value;
					this.SendPropertyChanged("logonserver");
					this.OnlogonserverChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_os", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string os
		{
			get
			{
				return this._os;
			}
			set
			{
				if ((this._os != value))
				{
					this.OnosChanging(value);
					this.SendPropertyChanging();
					this._os = value;
					this.SendPropertyChanged("os");
					this.OnosChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.WebTrackerEvents")]
	public partial class WebTrackerEvent : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private System.DateTime _DateTime;
		
		private string _Username;
		
		private string _IP;
		
		private string _ComputerName;
		
		private string _OS;
		
		private string _Browser;
		
		private string _Details;
		
		private string _EventType;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnDateTimeChanging(System.DateTime value);
    partial void OnDateTimeChanged();
    partial void OnUsernameChanging(string value);
    partial void OnUsernameChanged();
    partial void OnIPChanging(string value);
    partial void OnIPChanged();
    partial void OnComputerNameChanging(string value);
    partial void OnComputerNameChanged();
    partial void OnOSChanging(string value);
    partial void OnOSChanged();
    partial void OnBrowserChanging(string value);
    partial void OnBrowserChanged();
    partial void OnDetailsChanging(string value);
    partial void OnDetailsChanged();
    partial void OnEventTypeChanging(string value);
    partial void OnEventTypeChanged();
    #endregion
		
		public WebTrackerEvent()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DateTime", DbType="DateTime NOT NULL")]
		public System.DateTime DateTime
		{
			get
			{
				return this._DateTime;
			}
			set
			{
				if ((this._DateTime != value))
				{
					this.OnDateTimeChanging(value);
					this.SendPropertyChanging();
					this._DateTime = value;
					this.SendPropertyChanged("DateTime");
					this.OnDateTimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Username", DbType="NVarChar(50)")]
		public string Username
		{
			get
			{
				return this._Username;
			}
			set
			{
				if ((this._Username != value))
				{
					this.OnUsernameChanging(value);
					this.SendPropertyChanging();
					this._Username = value;
					this.SendPropertyChanged("Username");
					this.OnUsernameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IP", DbType="NVarChar(10)")]
		public string IP
		{
			get
			{
				return this._IP;
			}
			set
			{
				if ((this._IP != value))
				{
					this.OnIPChanging(value);
					this.SendPropertyChanging();
					this._IP = value;
					this.SendPropertyChanged("IP");
					this.OnIPChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ComputerName", DbType="NVarChar(50)")]
		public string ComputerName
		{
			get
			{
				return this._ComputerName;
			}
			set
			{
				if ((this._ComputerName != value))
				{
					this.OnComputerNameChanging(value);
					this.SendPropertyChanging();
					this._ComputerName = value;
					this.SendPropertyChanged("ComputerName");
					this.OnComputerNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_OS", DbType="NVarChar(50)")]
		public string OS
		{
			get
			{
				return this._OS;
			}
			set
			{
				if ((this._OS != value))
				{
					this.OnOSChanging(value);
					this.SendPropertyChanging();
					this._OS = value;
					this.SendPropertyChanged("OS");
					this.OnOSChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Browser", DbType="NVarChar(50)")]
		public string Browser
		{
			get
			{
				return this._Browser;
			}
			set
			{
				if ((this._Browser != value))
				{
					this.OnBrowserChanging(value);
					this.SendPropertyChanging();
					this._Browser = value;
					this.SendPropertyChanged("Browser");
					this.OnBrowserChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Details", DbType="NVarChar(MAX)")]
		public string Details
		{
			get
			{
				return this._Details;
			}
			set
			{
				if ((this._Details != value))
				{
					this.OnDetailsChanging(value);
					this.SendPropertyChanging();
					this._Details = value;
					this.SendPropertyChanged("Details");
					this.OnDetailsChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EventType", DbType="NVarChar(50)")]
		public string EventType
		{
			get
			{
				return this._EventType;
			}
			set
			{
				if ((this._EventType != value))
				{
					this.OnEventTypeChanging(value);
					this.SendPropertyChanging();
					this._EventType = value;
					this.SendPropertyChanged("EventType");
					this.OnEventTypeChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Tickets")]
	public partial class Ticket : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Title;
		
		private string _Status;
		
		private string _AssignedTo;
		
		private string _ShowTo;
		
		private string _Priority;
		
		private bool _Faq;
		
		private string _Archive;
		
		private bool _HideAssignedTo;
		
		private string _ReadBy;
		
		private EntitySet<Note> _Notes;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnTitleChanging(string value);
    partial void OnTitleChanged();
    partial void OnStatusChanging(string value);
    partial void OnStatusChanged();
    partial void OnAssignedToChanging(string value);
    partial void OnAssignedToChanged();
    partial void OnShowToChanging(string value);
    partial void OnShowToChanged();
    partial void OnPriorityChanging(string value);
    partial void OnPriorityChanged();
    partial void OnFaqChanging(bool value);
    partial void OnFaqChanged();
    partial void OnArchiveChanging(string value);
    partial void OnArchiveChanged();
    partial void OnHideAssignedToChanging(bool value);
    partial void OnHideAssignedToChanged();
    partial void OnReadByChanging(string value);
    partial void OnReadByChanged();
    #endregion
		
		public Ticket()
		{
			this._Notes = new EntitySet<Note>(new Action<Note>(this.attach_Notes), new Action<Note>(this.detach_Notes));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Title", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string Title
		{
			get
			{
				return this._Title;
			}
			set
			{
				if ((this._Title != value))
				{
					this.OnTitleChanging(value);
					this.SendPropertyChanging();
					this._Title = value;
					this.SendPropertyChanged("Title");
					this.OnTitleChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Status", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				if ((this._Status != value))
				{
					this.OnStatusChanging(value);
					this.SendPropertyChanging();
					this._Status = value;
					this.SendPropertyChanged("Status");
					this.OnStatusChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AssignedTo", DbType="NVarChar(MAX)")]
		public string AssignedTo
		{
			get
			{
				return this._AssignedTo;
			}
			set
			{
				if ((this._AssignedTo != value))
				{
					this.OnAssignedToChanging(value);
					this.SendPropertyChanging();
					this._AssignedTo = value;
					this.SendPropertyChanged("AssignedTo");
					this.OnAssignedToChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ShowTo", DbType="NVarChar(MAX)")]
		public string ShowTo
		{
			get
			{
				return this._ShowTo;
			}
			set
			{
				if ((this._ShowTo != value))
				{
					this.OnShowToChanging(value);
					this.SendPropertyChanging();
					this._ShowTo = value;
					this.SendPropertyChanged("ShowTo");
					this.OnShowToChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Priority", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string Priority
		{
			get
			{
				return this._Priority;
			}
			set
			{
				if ((this._Priority != value))
				{
					this.OnPriorityChanging(value);
					this.SendPropertyChanging();
					this._Priority = value;
					this.SendPropertyChanged("Priority");
					this.OnPriorityChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Faq", DbType="Bit NOT NULL")]
		public bool Faq
		{
			get
			{
				return this._Faq;
			}
			set
			{
				if ((this._Faq != value))
				{
					this.OnFaqChanging(value);
					this.SendPropertyChanging();
					this._Faq = value;
					this.SendPropertyChanged("Faq");
					this.OnFaqChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Archive", DbType="NVarChar(MAX)")]
		public string Archive
		{
			get
			{
				return this._Archive;
			}
			set
			{
				if ((this._Archive != value))
				{
					this.OnArchiveChanging(value);
					this.SendPropertyChanging();
					this._Archive = value;
					this.SendPropertyChanged("Archive");
					this.OnArchiveChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_HideAssignedTo", DbType="Bit NOT NULL")]
		public bool HideAssignedTo
		{
			get
			{
				return this._HideAssignedTo;
			}
			set
			{
				if ((this._HideAssignedTo != value))
				{
					this.OnHideAssignedToChanging(value);
					this.SendPropertyChanging();
					this._HideAssignedTo = value;
					this.SendPropertyChanged("HideAssignedTo");
					this.OnHideAssignedToChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ReadBy", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string ReadBy
		{
			get
			{
				return this._ReadBy;
			}
			set
			{
				if ((this._ReadBy != value))
				{
					this.OnReadByChanging(value);
					this.SendPropertyChanging();
					this._ReadBy = value;
					this.SendPropertyChanged("ReadBy");
					this.OnReadByChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Ticket_Note", Storage="_Notes", ThisKey="Id", OtherKey="TicketId")]
		public EntitySet<Note> Notes
		{
			get
			{
				return this._Notes;
			}
			set
			{
				this._Notes.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Notes(Note entity)
		{
			this.SendPropertyChanging();
			entity.Ticket = this;
		}
		
		private void detach_Notes(Note entity)
		{
			this.SendPropertyChanging();
			entity.Ticket = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Note")]
	public partial class Note : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _TicketId;
		
		private int _Id;
		
		private System.DateTime _DateTime;
		
		private string _Username;
		
		private string _Content;
		
		private bool _Hide;
		
		private EntitySet<NoteFile> _NoteFiles;
		
		private EntityRef<Ticket> _Ticket;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnTicketIdChanging(int value);
    partial void OnTicketIdChanged();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnDateTimeChanging(System.DateTime value);
    partial void OnDateTimeChanged();
    partial void OnUsernameChanging(string value);
    partial void OnUsernameChanged();
    partial void OnContentChanging(string value);
    partial void OnContentChanged();
    partial void OnHideChanging(bool value);
    partial void OnHideChanged();
    #endregion
		
		public Note()
		{
			this._NoteFiles = new EntitySet<NoteFile>(new Action<NoteFile>(this.attach_NoteFiles), new Action<NoteFile>(this.detach_NoteFiles));
			this._Ticket = default(EntityRef<Ticket>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TicketId", DbType="Int NOT NULL")]
		public int TicketId
		{
			get
			{
				return this._TicketId;
			}
			set
			{
				if ((this._TicketId != value))
				{
					if (this._Ticket.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnTicketIdChanging(value);
					this.SendPropertyChanging();
					this._TicketId = value;
					this.SendPropertyChanged("TicketId");
					this.OnTicketIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DateTime", DbType="DateTime NOT NULL")]
		public System.DateTime DateTime
		{
			get
			{
				return this._DateTime;
			}
			set
			{
				if ((this._DateTime != value))
				{
					this.OnDateTimeChanging(value);
					this.SendPropertyChanging();
					this._DateTime = value;
					this.SendPropertyChanged("DateTime");
					this.OnDateTimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Username", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string Username
		{
			get
			{
				return this._Username;
			}
			set
			{
				if ((this._Username != value))
				{
					this.OnUsernameChanging(value);
					this.SendPropertyChanging();
					this._Username = value;
					this.SendPropertyChanged("Username");
					this.OnUsernameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Content", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string Content
		{
			get
			{
				return this._Content;
			}
			set
			{
				if ((this._Content != value))
				{
					this.OnContentChanging(value);
					this.SendPropertyChanging();
					this._Content = value;
					this.SendPropertyChanged("Content");
					this.OnContentChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Hide", DbType="Bit NOT NULL")]
		public bool Hide
		{
			get
			{
				return this._Hide;
			}
			set
			{
				if ((this._Hide != value))
				{
					this.OnHideChanging(value);
					this.SendPropertyChanging();
					this._Hide = value;
					this.SendPropertyChanged("Hide");
					this.OnHideChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Note_NoteFile", Storage="_NoteFiles", ThisKey="Id", OtherKey="NoteId")]
		public EntitySet<NoteFile> NoteFiles
		{
			get
			{
				return this._NoteFiles;
			}
			set
			{
				this._NoteFiles.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Ticket_Note", Storage="_Ticket", ThisKey="TicketId", OtherKey="Id", IsForeignKey=true, DeleteOnNull=true, DeleteRule="CASCADE")]
		public Ticket Ticket
		{
			get
			{
				return this._Ticket.Entity;
			}
			set
			{
				Ticket previousValue = this._Ticket.Entity;
				if (((previousValue != value) 
							|| (this._Ticket.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Ticket.Entity = null;
						previousValue.Notes.Remove(this);
					}
					this._Ticket.Entity = value;
					if ((value != null))
					{
						value.Notes.Add(this);
						this._TicketId = value.Id;
					}
					else
					{
						this._TicketId = default(int);
					}
					this.SendPropertyChanged("Ticket");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_NoteFiles(NoteFile entity)
		{
			this.SendPropertyChanging();
			entity.Note = this;
		}
		
		private void detach_NoteFiles(NoteFile entity)
		{
			this.SendPropertyChanging();
			entity.Note = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.NoteFiles")]
	public partial class NoteFile : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _NoteId;
		
		private int _Id;
		
		private System.Data.Linq.Binary _Data;
		
		private string _ContentType;
		
		private EntityRef<Note> _Note;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnNoteIdChanging(int value);
    partial void OnNoteIdChanged();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnDataChanging(System.Data.Linq.Binary value);
    partial void OnDataChanged();
    partial void OnContentTypeChanging(string value);
    partial void OnContentTypeChanged();
    #endregion
		
		public NoteFile()
		{
			this._Note = default(EntityRef<Note>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NoteId", DbType="Int NOT NULL")]
		public int NoteId
		{
			get
			{
				return this._NoteId;
			}
			set
			{
				if ((this._NoteId != value))
				{
					if (this._Note.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnNoteIdChanging(value);
					this.SendPropertyChanging();
					this._NoteId = value;
					this.SendPropertyChanged("NoteId");
					this.OnNoteIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Data", DbType="VarBinary(MAX) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary Data
		{
			get
			{
				return this._Data;
			}
			set
			{
				if ((this._Data != value))
				{
					this.OnDataChanging(value);
					this.SendPropertyChanging();
					this._Data = value;
					this.SendPropertyChanged("Data");
					this.OnDataChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ContentType", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string ContentType
		{
			get
			{
				return this._ContentType;
			}
			set
			{
				if ((this._ContentType != value))
				{
					this.OnContentTypeChanging(value);
					this.SendPropertyChanging();
					this._ContentType = value;
					this.SendPropertyChanged("ContentType");
					this.OnContentTypeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Note_NoteFile", Storage="_Note", ThisKey="NoteId", OtherKey="Id", IsForeignKey=true, DeleteOnNull=true, DeleteRule="CASCADE")]
		public Note Note
		{
			get
			{
				return this._Note.Entity;
			}
			set
			{
				Note previousValue = this._Note.Entity;
				if (((previousValue != value) 
							|| (this._Note.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Note.Entity = null;
						previousValue.NoteFiles.Remove(this);
					}
					this._Note.Entity = value;
					if ((value != null))
					{
						value.NoteFiles.Add(this);
						this._NoteId = value.Id;
					}
					else
					{
						this._NoteId = default(int);
					}
					this.SendPropertyChanged("Note");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
