import {Md5} from 'ts-md5';
import { environment } from '../../environments/environment';

export class AppUser {

    id = null;
    userName = '';
    email = '';
    phoneNumber = null;
    lockoutEnd = null;
    cleanPassword = '';
    loginNT = '';

    userRoles = [];
    userClaims = [];

    get isLockedout(): boolean {
        if (this.lockoutEnd == null) {
            return false;
        }
        const end = new Date(this.lockoutEnd);
        const now = new Date();
        return end >= now;

    }

    get givenName(): string {
        const claim = this.userClaims.find(c => c.claimType === 'given_name');
        if (!claim) {
            return '';
        }
        return claim.claimValue;
    }

    set givenName(value: string) {
        let claim = this.userClaims.find(c => c.claimType === 'given_name');
        if (!claim) {
            this.userClaims.push({claimType: 'given_name', claimValue: ''});
            claim = this.userClaims.filter(c => c.claimType === 'given_name');
        }
        claim.claimValue = value;
    }

    get avatarImageUrl() {
        return environment.IDENTITY_SERVER_URL + '/user-photo/' + this.id;
    }

    get userNameAndEmail(): string {
        if (this.userName === this.email) {
            return this.userName;
        }
        return this.userName + '/' + this.email;
    }

    static getAvatarImageUrl(email: string) {
        const hash = Md5.hashAsciiStr(email);
        return 'https://www.gravatar.com/avatar/' + hash + '?d=mp';
    }

    public static fromClaims(claims): AppUser {
        const user = new AppUser();
        if (!claims) { return user; }
        user.email = claims['email'];
        user.id = claims['sub'];
        user.userClaims.push({ claimType: 'given_name', claimValue: claims['given_name']});
        return user;
    }

}

export class AppRole {

    id = null;
    name = '';
    description = '';
    roleClaims = [];
    isSystemRole: boolean;

}
