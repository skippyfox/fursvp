import * as React from 'react';
import { Modal, ModalHeader, ModalBody, ModalFooter, Button, FormGroup, Form, Label, Input, NavLink } from 'reactstrap';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as UserStore from '../store/UserStore';
import { Link } from 'react-router-dom';

// At runtime, Redux will merge together...
type UserProps =
    UserStore.UserState // ... state we've requested from the Redux store
    & typeof UserStore.actionCreators // ... plus action creators we've requested
    & RouteComponentProps<{ eventId: string, memberId: string }>; // ... plus incoming routing parameters


class User extends React.PureComponent<UserProps> {
    constructor(props: UserProps) {
        super(props);

        this.sendVerificationCode = this.sendVerificationCode.bind(this);
        this.sendVerificationEmail = this.sendVerificationEmail.bind(this);
    }

    public render() {
        return (            
            <React.Fragment>
                <NavLink tag={Link} onClick={this.props.openUserInfoModal} className="text-dark">{this.props.verifiedEmail == undefined ? "Log In" : "Account"}</NavLink>
                <Modal isOpen={this.props.loginModalIsOpen} toggle={this.props.toggleModal}>
                    <ModalHeader toggle={this.props.toggleModal}>Log In</ModalHeader>
                    <ModalBody>
                        <Form>
                            <FormGroup>
                                <Label for="exampleEmail">Email</Label>
                                <Input type="email" name="email" id="loginModalEmailAddress" disabled={this.props.verificationEmailIsSending} />
                            </FormGroup>
                        </Form>
                    </ModalBody>
                    <ModalFooter>
                        <Button color="primary" onClick={this.sendVerificationEmail} disabled={this.props.verificationEmailIsSending}>Send Verification Email</Button>{' '}
                        <Button color="secondary" onClick={this.props.toggleModal} disabled={this.props.verificationEmailIsSending}>Cancel</Button>
                    </ModalFooter>
                </Modal>
                <Modal isOpen={this.props.verifyModalIsOpen} toggle={this.props.toggleModal}>
                    <ModalHeader toggle={this.props.toggleModal}>Verify Email</ModalHeader>
                    <ModalBody>
                        Enter the verification code we sent to <code>{this.props.emailBeingVerified}</code>.
                        <Form>
                            <FormGroup>
                                <Label for="exampleEmail">Verification Code</Label>
                                <Input type="email" name="email" id="verifyModalCode" disabled={this.props.verificationCodeIsSending} />
                            </FormGroup>
                        </Form>
                    </ModalBody>
                    <ModalFooter>
                        <Button color="primary" onClick={this.sendVerificationCode} disabled={this.props.verificationCodeIsSending}>Verify</Button>{' '}
                        <Button color="secondary" onClick={this.props.toggleModal} disabled={this.props.verificationCodeIsSending}>Cancel</Button>
                    </ModalFooter>
                </Modal>
                <Modal isOpen={this.props.userInfoModalIsOpen} toggle={this.props.toggleModal}>
                    <ModalHeader toggle={this.props.toggleModal}>{this.props.verifiedEmail === undefined ? "Not Logged In" : "Logged In"}</ModalHeader>
                    <ModalBody>
                        {this.props.verifiedEmail === undefined
                            ? <>You are not logged in.</>
                            : <>You are verified as <code>{this.props.verifiedEmail}</code>.</>
                        }
                    </ModalBody>
                    <ModalFooter>
                        <Button color="primary" onClick={this.props.toggleModal} disabled={this.props.verificationCodeIsSending}>Continue</Button>{' '}
                        {this.props.verifiedEmail === undefined ?
                            <Button color="warning" onClick={this.props.openLoginModal} disabled={this.props.verificationCodeIsSending}>Log In</Button>
                            :
                            <Button outline color="danger" onClick={this.props.logOut} disabled={this.props.verificationCodeIsSending}>Log Out</Button>
                        }
                    </ModalFooter>
                </Modal>
            </React.Fragment>
        );
    }

    private sendVerificationEmail() {
        var emailAddress = (document.getElementById('loginModalEmailAddress') as HTMLInputElement).value;
        this.props.sendVerificationEmail(emailAddress);
    }

    private sendVerificationCode() {
        var code = (document.getElementById('verifyModalCode') as HTMLInputElement).value;
        this.props.checkVerificationCode(this.props.emailBeingVerified, code);
    }
}

export default connect(
    (state: ApplicationState) => state.user, // Selects which state properties are merged into the component's props
    UserStore.actionCreators // Selects which action creators are merged into the component's props
)(User as any);