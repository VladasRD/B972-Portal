import { environment } from '../../environments/environment';
import { ColorUtil } from '../common/color-util';
import { CrossLink } from './crosslink';

export class ContentData {
    contentUId: string;
    json: string;
}


export class ContentTag {
    contentUId: string;
    tag: string;
    bgColorCss: string;

    setTagBgCss() {
        this.bgColorCss = ColorUtil.getBgColorClass(this.tag);
    }
}

export class ContentHead {
    contentUId: string;
    name: string;
    abstract: string;
    kind: string;
    location: string;
    thumbFilePath: string;
    externalLinkUrl: string;
    customInfo: ContentCustomInfo;
    displayOrder: number;

    data: ContentData;

    createDate: Date;
    contentDate: Date;

    publishUntil: Date;
    publishAfter: Date;

    tags: ContentTag[];
    crossLinks: CrossLink[];

    get isPublished() {
        const now = new Date();
        if (this.publishUntil && this.publishUntil < now) {
            return false;
        }
        if (this.publishAfter && this.publishAfter > now) {
            return false;
        }
        if (!this.publishAfter) {
            return false;
        }
        return true;
    }

    get thumbUrl() {
        if (!this.thumbFilePath) {
            return '';
        }
        return environment.IDENTITY_SERVER_URL + this.thumbFilePath;
    }

    setTagsBgCss() {
        this.tags.forEach(t => { t.bgColorCss = ColorUtil.getBgColorClass(t.tag); });
    }

    get pageView(): string {
        if (!this['pageViewCount']) {
            return '0';
        }
        const c = parseInt(this['pageViewCount'].count, 10);
        if (c > 100) {
            return Math.round((c / 100)) + 'k';
        }
        return c + '';
    }

}

export class ContentCustomInfo {
    contentUId: string;
    text1: string;
    text2: string;
    text3: string;
    text4: string;
    number1: number;
    number2: number;
    number3: number;
    number4: number;
    date1: Date;
    date2: Date;
    date3: Date;
    date4: Date;
}
