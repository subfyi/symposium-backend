using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class UserExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static User ToUser(this user a) => new User
    {
        id = a.id,
        name = a.first_name,
        surname = a.last_name,
        role = a.type == UserType.ADMIN ? 9 : 1,
        address = a.address,
        institution = a.institution,
        faculty = a.faculty,
        department = a.department,
        iletisim = a.iletisim,
        u_title = a.u_title,
        u_type = a.u_type,
        gender = a.gender,
        first_name = a.first_name,
        last_name = a.last_name,
        email = a.email,
        type = a.type,
        created_at = a.created_at,
        is_password_present = a.password != null,
        password_updated_at = a.password_set_at
    };
}
