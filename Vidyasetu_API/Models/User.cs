﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Vidyasetu_API.Models;

public partial class User
{
    public long Id { get; set; }

    public string Firstname { get; set; }

    public string Lastname { get; set; }

    public string Email { get; set; }

    public string Mobile { get; set; }

    public bool? ActiveFlag { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string Password { get; set; }

    public string Role { get; set; }

    public virtual ICollection<DeviceDetail> DeviceDetails { get; set; } = new List<DeviceDetail>();

    public virtual ICollection<UserRequestPreference> UserRequestPreferences { get; set; } = new List<UserRequestPreference>();
}