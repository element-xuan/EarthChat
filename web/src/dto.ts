export interface GetUserDto {
    id: string;
    account: string;
    avatar: string;
    name: string;
    onLine: boolean;
}

export interface ChatGroupDto {
    id: string;
    name: string;
    avatar: string;
    description: string;
    group: boolean;
    default: boolean;
    lastMessage: string;
    creator:string;
    creationTime: Date;
    type:string;   
}

export interface CreateGroupDto {
    name: string;
    avatar: string;
    description: string;
}

export interface FriendRegistrationInput {
    description: string;
    beAppliedForId: string;
}